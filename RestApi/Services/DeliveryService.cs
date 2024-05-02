using RestApi.DTOs;
using RestApi.Exceptions;
using RestApi.Models;
using RestApi.Repositories;

namespace RestApi.Services;

public interface IDeliveryService
{
    public Task<Delivery?> AddDelivery(DeliveryDTO dto);
    public Task<Delivery?> AddDeliveryProcedurally(DeliveryDTO dto);
}

public class DeliveryService(
    IDeliveryRepository deliveryRepository,
    IProductRepository productRepository,
    IWarehouseRepository warehouseRepository,
    IOrderRepository orderRepository
) : IDeliveryService
{
    public async Task<Delivery?> AddDelivery(DeliveryDTO dto)
    {
        if (dto.Amount < 1) throw new BadDataException("Amount cannot be less than 1");

        var product = await productRepository.GetProductById(dto.IdProduct);
        if (product == null) throw new NotFoundException($"Product with ID: {dto.IdProduct} was not found");
        var price = product.Price * dto.Amount;

        var warehouse = await warehouseRepository.GetWarehouseById(dto.IdWarehouse);
        if (warehouse == null) throw new NotFoundException($"Warehouse with ID: {dto.IdWarehouse} was not found");

        var order = await orderRepository.GetOrderByProductIdAndAmount(dto.IdProduct, dto.Amount);
        if (order == null)
            throw new NotFoundException(
                $"Order with Product ID: {dto.IdProduct}, and amount: {dto.Amount} was not found");
        var idOrder = order.IdOrder;
        if (order.CreatedAt > dto.CreatedAt)
            throw new BadDataException($"Order with ID: {idOrder} was created LATER than this request");

        var delivery = await deliveryRepository.GetDeliveryByOrderId(dto.IdProduct);
        if (delivery != null)
            throw new AlreadyProcessedException($"Order with ID: {idOrder} is already being processed");

        return await deliveryRepository.AddDelivery(
            dto.IdWarehouse,
            dto.IdProduct,
            idOrder,
            dto.Amount,
            price,
            dto.CreatedAt);
    }

    /// <exception cref="BadDataException">Thrown if the stored procedure returns a 21 or 26 Error Code.</exception>
    /// <exception cref="NotFoundException">Thrown if the stored procedure returns a 22, 23 or 24 Error Code.</exception>
    /// <exception cref="AlreadyProcessedException">Thrown if the stored procedure returns a 25 Error Code.</exception>
    public async Task<Delivery?> AddDeliveryProcedurally(DeliveryDTO dto)
    {
        return await deliveryRepository.AddDeliveryProcedurally(
            dto.IdWarehouse,
            dto.IdProduct,
            dto.Amount,
            dto.CreatedAt);
    }
}
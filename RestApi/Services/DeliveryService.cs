using RestApi.DTOs;
using RestApi.Exceptions;
using RestApi.Models;
using RestApi.Repositories;

namespace RestApi.Services;

public interface IDeliveryService
{
    public Delivery? AddDelivery(DeliveryDTO dto);
}

public class DeliveryService(
    IDeliveryRepository deliveryRepository, 
    IProductRepository productRepository, 
    IWarehouseRepository warehouseRepository,
    IOrderRepository orderRepository
    ) : IDeliveryService
{
    public Delivery? AddDelivery(DeliveryDTO dto)
    {
        if (dto.Amount < 1) throw new BadDataException($"Amount cannot be less than 1");
        
        var product = productRepository.GetProductById(dto.IdProduct);
        if (product == null) throw new NotFoundException($"Product with ID: {dto.IdProduct} was not found");
        var price = product.Price * dto.Amount;

        var warehouse = warehouseRepository.GetWarehouseById(dto.IdWarehouse);
        if (warehouse == null) throw new NotFoundException($"Warehouse with ID: {dto.IdWarehouse} was not found");

        var order = orderRepository.GetOrderByProductIdAndAmount(dto.IdProduct, dto.Amount);
        if (order == null) throw new NotFoundException($"Order with Product ID: {dto.IdProduct}, and amount: {dto.Amount} was not found");
        var idOrder = order.IdOrder;
        if (order.CreatedAt > dto.CreatedAt) throw new BadDataException($"Order with ID: {idOrder} was created LATER than this request");

        // if this.getDeliveryBy??(dto.IdWarehouse + dto.IdProduct + idOrder) - exists throw new AlreadyProcessedException()
        var delivery = deliveryRepository.GetDeliveryByOrderId(dto.IdProduct);
        if(delivery != null) throw new AlreadyProcessedException($"Order with ID: {idOrder} is already being processed");
        
        return deliveryRepository.AddDelivery(dto.IdWarehouse, dto.IdProduct, idOrder, dto.Amount, price, dto.CreatedAt);
    }
}
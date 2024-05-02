using RestApi.DTOs;
using RestApi.Models;
using RestApi.Repositories;

namespace RestApi.Services;

public interface IDeliveryService
{
    public Delivery? AddDelivery(DeliveryDTO dto);
}

public class DeliveryService(IDeliveryRepository deliveryRepository) : IDeliveryService
{
    public Delivery? AddDelivery(DeliveryDTO dto)
    {
        // Example Flow:
        
        // product = productService.getProductById(dto.IdProduct)
        // if !product throw new NotFoundException()
        // price = product.getPrice() * dto.Amount
        
        // warehouse = warehouseService.getWarehouseById(dto.IdWarehouse)
        // if !warehouse throw new NotFoundException()
        
        // order = orderService.getOrderByProductIdAndAmount(dto.IdProduct, dto.Amount)
        // if !order throw new NotFoundException()
        // if order.getCreatedAt() > dto.CreatedAt throw new BadDateException()
        // idOrder = order.getId()

        // if this.getDeliveryBy??(dto.IdWarehouse + dto.IdProduct + idOrder) - exists throw new AlreadyHandledException()
        
        //Temporary
        const int idOrder = 1;
        var price = (decimal)(25.5 * dto.Amount);
        
        return deliveryRepository.AddDelivery(dto.IdWarehouse, dto.IdProduct, idOrder, dto.Amount, price, dto.CreatedAt);
    }
}
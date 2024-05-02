using Microsoft.AspNetCore.Mvc;
using RestApi.DTOs;
using RestApi.Exceptions;
using RestApi.Models.ResponseModels;
using RestApi.Services;

namespace RestApi.Controllers;

[ApiController]
[Route("/api/warehouses")]
public class WarehousesController(IDeliveryService service) : ControllerBase
{
    [HttpPost("")]
    [ProducesResponseType(typeof(CreatedResponseModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(NoChangesResponseModel), StatusCodes.Status304NotModified)]
    [ProducesResponseType(typeof(BadRequestResponseModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundResponseModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ConflictResponseModel), StatusCodes.Status409Conflict)]
    public IActionResult CreateDelivery([FromBody] DeliveryDTO dto)
    {
        try
        {
            var createdDelivery = service.AddDelivery(dto);
            return createdDelivery != null
                ? Created("",
                    new CreatedResponseModel(
                        $"Registered a new Delivery with ID: {createdDelivery.IdProductWarehouse}",
                        createdDelivery))
                : Conflict(new ConflictResponseModel("Could not register a new Delivery with given parameters"));
        }
        catch (AlreadyHandledException e)
        {
            return StatusCode(StatusCodes.Status304NotModified, new NoChangesResponseModel(e.Message));
        }
        catch (BadDateException e)
        {
            return BadRequest(new BadRequestResponseModel(e.Message));
        }
        catch (NotFoundException e)
        {
            return NotFound(new NotFoundResponseModel(e.Message));
        }
    }
}
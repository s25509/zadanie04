using Microsoft.AspNetCore.Mvc;

namespace RestApi.Animals;

[ApiController]
[Route("/api/animals")]
public class AnimalController(IAnimalService service) : ControllerBase
{
    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponseModel), StatusCodes.Status400BadRequest)]
    public IActionResult GetAllAnimals([FromQuery] string? orderBy)
    {
        orderBy ??= "name";
        if (!AnimalRepository.ValidOrderParameters.Contains(orderBy))
        {
            return BadRequest(new BadRequestResponseModel($"Cannot sort by: {orderBy}"));
        }

        var animals = service.GetAllAnimals(orderBy);
        return Ok(animals);
    }

    [HttpPost("")]
    [ProducesResponseType(typeof(CreatedResponseModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ConflictResponseModel), StatusCodes.Status409Conflict)]
    public IActionResult CreateAnimal([FromBody] CreateAnimalDTO dto)
    {
        var createdAnimal = service.AddAnimal(dto);
        return createdAnimal != null
            ? Created("",
                new CreatedResponseModel($"Created a new Animal with id: {createdAnimal.IdAnimal}", createdAnimal))
            : Conflict(new ConflictResponseModel($"Could not create a new Animal"));
    }

    [HttpPut("{idAnimal:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ConflictResponseModel), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(NotFoundResponseModel), StatusCodes.Status404NotFound)]
    public IActionResult UpdateAnimal([FromRoute] int idAnimal, [FromBody] UpdateAnimalDTO dto)
    {
        // Check if animal exists
        var animal = service.GetAnimalById(idAnimal);
        if (animal == null)
        {
            return NotFound(new NotFoundResponseModel($"Animal with id: {idAnimal} not found"));
        }

        var updatedAnimal = service.UpdateAnimal(idAnimal, dto);
        return updatedAnimal != null
            ? Ok(new { message = $"Updated Animal with id: {idAnimal}", animal = updatedAnimal })
            : Conflict(new ConflictResponseModel($"Could not update Animal with id: {idAnimal}"));
    }

    [HttpDelete("{idAnimal:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ConflictResponseModel), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(NotFoundResponseModel), StatusCodes.Status404NotFound)]
    public IActionResult DeleteAnimal([FromRoute] int idAnimal)
    {
        // Check if animal exists
        var animal = service.GetAnimalById(idAnimal);
        if (animal == null)
        {
            return NotFound(new NotFoundResponseModel($"Animal with id: {idAnimal} not found"));
        }

        var deletedAnimal = service.DeleteAnimal(idAnimal);
        return deletedAnimal != null
            ? Ok(new { message = $"Removed Animal with id: {idAnimal}", animal = deletedAnimal })
            : Conflict(new ConflictResponseModel($"Could not remove Animal with id: {idAnimal}"));
    }
}
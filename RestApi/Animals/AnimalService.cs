namespace RestApi.Animals;

public interface IAnimalService
{
    public IEnumerable<Animal> GetAllAnimals(string orderBy);
    public Animal? GetAnimalById(int idAnimal);
    public Animal? AddAnimal(CreateAnimalDTO dto);
    public Animal? UpdateAnimal(int idAnimal, UpdateAnimalDTO dto);
    public Animal? DeleteAnimal(int idAnimal);
}

public class AnimalService(IAnimalRepository animalRepository) : IAnimalService
{
    public IEnumerable<Animal> GetAllAnimals(string orderBy)
    {
        return animalRepository.FetchAllAnimals(orderBy);
    }

    public Animal? GetAnimalById(int idAnimal)
    {
        return animalRepository.FetchAnimalById(idAnimal);
    }

    public Animal? AddAnimal(CreateAnimalDTO dto)
    {
        return animalRepository.CreateAnimal(dto.Name,dto.Description,dto.Category,dto.Area);
    }

    public Animal? UpdateAnimal(int idAnimal, UpdateAnimalDTO dto)
    {
        return animalRepository.UpdateAnimal(idAnimal, dto.Name,dto.Description,dto.Category,dto.Area);
    }

    public Animal? DeleteAnimal(int idAnimal)
    {
        return animalRepository.RemoveAnimal(idAnimal);
    }
}
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RestApi.Animals;

public class CreateAnimalDTO
{
    [Required]
    [MaxLength(200)]
    [DefaultValue("The animal's name")]
    public string Name { get; set; }
    [MaxLength(200)]
    [DefaultValue("Some detailed description of the animal")]
    public string? Description { get; set; }
    [Required]
    [MaxLength(200)]
    [DefaultValue("eg. Bird, Fish, Mammal...")]
    public string Category { get; set; }
    [Required]
    [MaxLength(200)]
    [DefaultValue("The animal's area of occurrence")]
    public string Area { get; set; }
}

public class UpdateAnimalDTO
{
    [MaxLength(200)]
    [DefaultValue("Updated animal's name")]
    public string? Name { get; set; }
    [MaxLength(200)]
    [DefaultValue("Updated description of the animal")]
    public string? Description { get; set; }
    [MaxLength(200)]
    [DefaultValue("Updated category")]
    public string? Category { get; set; }
    [MaxLength(200)]
    [DefaultValue("Updated area of occurrence")]
    public string? Area { get; set; }
}
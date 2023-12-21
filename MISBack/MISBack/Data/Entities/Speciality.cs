using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Entities;

public class Speciality
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public DateTime CreateTime { get; set; }
    
    [Required]
    [MinLength(1)]
    public string Name { get; set; }
}
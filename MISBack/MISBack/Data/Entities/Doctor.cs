using System.ComponentModel.DataAnnotations;
using MISBack.Data.Enums;

namespace MISBack.Data.Entities;

public class Doctor
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public DateTime CreateTime { get; set; }
    
    [Required]
    [MinLength(1)]
    public string Name { get; set; }

    public DateTime? BirthDate { get; set; }
    
    [Required]
    public Gender Gender { get; set; }
    
    [Required]
    [MinLength(1)]
    [EmailAddress]
    public string Email { get; set; }
    
    [Phone]
    public string? Phone { get; set; }
    
    [Required]
    public Guid Speciality { get; set; }
}
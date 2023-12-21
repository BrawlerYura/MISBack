using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Models;

public class DoctorRegisterModel
{
    [Required]
    [MaxLength(1000)]
    [MinLength(1)]
    public string Name { get; set; }
    
    [Required]
    [MinLength(6)]
    public string Password { get; set; }
    
    [Required]
    [MinLength(1)]
    [EmailAddress]
    public string Email { get; set; }
    
    public DateTime? BirthDate { get; set; }
    
    [Required]
    public Gender Gender { get; set; }
    
    [Phone]
    public string? Phone { get; set; }
    
    [Required]
    public Guid Speciality { get; set; }
}
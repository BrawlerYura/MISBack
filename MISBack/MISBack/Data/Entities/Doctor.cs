using MISBack.Data.Enums;

namespace MISBack.Data.Entities;

public class Doctor
{
    public Guid Id { get; set; }
    
    public DateTime CreateTime { get; set; }
    
    public string Name { get; set; }

    public DateTime? BirthDate { get; set; }
    
    public Gender Gender { get; set; }
    
    public string Email { get; set; }
    
    public string Password { get; set; }
    
    public string? Phone { get; set; }
    
    public Guid Speciality { get; set; }
}
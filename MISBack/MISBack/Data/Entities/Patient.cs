using MISBack.Data.Enums;

namespace MISBack.Data.Entities;

public class Patient
{
    public Guid Id { get; set; }
    
    public DateTime CreateTime { get; set; }

    public string Name { get; set; }
    
    public string Email { get; set; }
    
    public DateTime? Birthday { get; set; }

    public Gender Gender { get; set; }
}
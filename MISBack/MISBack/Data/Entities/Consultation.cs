using MISBack.Data.Models;

namespace MISBack.Data.Entities;

public class Consultation
{
    public Guid Id { get; set; }
    
    public DateTime CreateTime { get; set; }
    
    public Guid InspectionId { get; set; }
    
    public Guid? SpecialityId { get; set; }
}
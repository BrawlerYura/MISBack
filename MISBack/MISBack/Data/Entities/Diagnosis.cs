using MISBack.Data.Enums;

namespace MISBack.Data.Entities;

public class Diagnosis
{
    public Guid Id { get; set; }
    
    public DateTime CreateTime { get; set; }
    
    public string Code { get; set; }
    
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    public DiagnosisType Type { get; set; }
    
    public Guid InspectionId { get; set; }
}
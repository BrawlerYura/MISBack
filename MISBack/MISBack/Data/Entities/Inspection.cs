using MISBack.Data.Enums;

namespace MISBack.Data.Entities;

public class Inspection
{
    public Guid Id { get; set; }
    
    public DateTime CreateTime { get; set; }
    
    public DateTime? Date {  get; set; }
    
    public string? Anamnesis { get; set; }
    
    public string? Complaints { get; set; }
    
    public string? Treatment { get; set; }
    
    public Conclusion? Conclusion { get; set; }
    
    public DateTime? NextVisitDate { get; set; }
    
    public DateTime? DeathDate { get; set; }
    
    public Guid? BaseInspectionId { get; set; }
    
    public Guid? PreviousInspectionId { get; set; }
    
    public Guid PatientId { get; set; }
    
    public Guid DoctorId { get; set; }
}
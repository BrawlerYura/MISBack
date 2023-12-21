using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Models;

public class InspectionCreateModel
{
    [Required]
    public DateTime Date {  get; set; }
    
    [Required]
    [MinLength(1)]
    [MaxLength(5000)]
    public string Anamnesis { get; set; }
    
    [Required]
    [MaxLength(5000)]
    [MinLength(1)]
    public string Complaints { get; set; }
    
    [Required]
    [MaxLength(5000)]
    [MinLength(1)]
    public string Treatment { get; set; }
    
    [Required]
    public Conclusion Conclusion {  get; set; } 
    
    public DateTime? NextVisitDate { get; set; }
    
    public DateTime? DeathDate { get; set; }
    
    public Guid? PreviousInspectionId { get; set; }
    
    [Required]
    [MinLength(1)]
    public List<DiagnosisCreateModel> Diagnoses { get; set; }
    
    public List<ConsultationCreateModel>? Consultations { get; set; }
}
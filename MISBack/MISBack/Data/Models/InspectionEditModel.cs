using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Models;

public class InspectionEditModel
{
    [MaxLength(5000)]
    public string? Anamnesis {  get; set; }
    
    [Required]
    [MaxLength(5000)]
    [MinLength(1)]
    public string Complaints { get; set; }
    
    [Required]
    [MaxLength(5000)]
    [MinLength(1)]
    public string Treatment { get; set; }
    
    [Required]
    public Conclusion Conclusion { get; set; }
    
    public DateTime? NextVisitDate { get; set; }
    
    public DateTime? DeathDate { get; set; }
    
    [Required]
    public List<DiagnosisCreateModel> Diagnoses { get; set; }
}
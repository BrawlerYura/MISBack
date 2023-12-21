using System.ComponentModel.DataAnnotations;
using MISBack.Data.Enums;

namespace MISBack.Data.Models;

public class InspectionPreviewModel
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public DateTime CreateTime { get; set; }
    
    public Guid? PreviousId { get; set; }
    
    [Required]
    public DateTime Date {  get; set; }
    
    [Required]
    public Conclusion Conclusion { get; set; }
    
    [Required]
    public Guid DoctorId { get; set; }
    
    [Required]
    [MinLength(1)]
    public string Doctor {  get; set; }
    
    [Required]
    public Guid PatientId { get; set; }
    
    [Required]
    [MinLength(1)]
    public string Patient { get; set; }
    
    [Required]
    public DiagnosisModel Diagnosis { get; set; }
    
    public bool? HasChain { get; set; }
    
    public bool? HasNested {  get; set; }
}
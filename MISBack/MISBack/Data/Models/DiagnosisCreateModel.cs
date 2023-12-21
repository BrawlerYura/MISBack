using System.ComponentModel.DataAnnotations;
using MISBack.Data.Enums;

namespace MISBack.Data.Models;

public class DiagnosisCreateModel
{
    [Required]
    public Guid IcdDiagnosisId { get; set; }
    
    [MaxLength(5000)]
    public string? Description { get; set; }
    
    [Required]
    public DiagnosisType Type { get; set; }
}
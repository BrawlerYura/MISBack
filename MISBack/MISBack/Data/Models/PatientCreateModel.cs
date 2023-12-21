using System.ComponentModel.DataAnnotations;
using MISBack.Data.Enums;

namespace MISBack.Data.Models;

public class PatientCreateModel
{
    [Required]
    [MinLength(1)]
    [MaxLength(1000)]
    public string Name {  get; set; }
    
    public DateTime? Birthday { get; set; }
    
    [Required]
    public Gender Gender { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Entities;

public class Icd10
{
    [Required]
    public Guid Id { get; set; }
    
    public string? Code { get; set; }
    
    public string? Name { get; set; }
    
    public Guid ParentId { get; set; }
}
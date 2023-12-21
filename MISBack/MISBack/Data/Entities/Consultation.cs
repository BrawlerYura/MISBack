using System.ComponentModel.DataAnnotations;
using MISBack.Data.Models;

namespace MISBack.Data.Entities;

public class Consultation
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public DateTime CreateTime { get; set; }
    
    [Required]
    public Guid InspectionId { get; set; }
    
    public SpecialityModel? Speciality { get; set; }
    
    public List<CommentModel>? Comments { get; set; }
}
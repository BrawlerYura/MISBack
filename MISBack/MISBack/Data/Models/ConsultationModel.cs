using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Models;

public class ConsultationModel
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public DateTime CreateTime { get; set; }
    
    public Guid? InspectionId { get; set; }
    
    public SpecialityModel? Speciality { get; set; }
    
    public List<CommentModel>? Comments { get; set; }
}
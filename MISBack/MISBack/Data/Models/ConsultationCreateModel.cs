using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Models;

public class ConsultationCreateModel
{
    [Required]
    public Guid SpecialityId { get; set; }
    
    [Required]
    public Guid InspectionId { get; set; }
    
    [Required]
    public InspectionCommentCreateModel Comment { get; set; }
}
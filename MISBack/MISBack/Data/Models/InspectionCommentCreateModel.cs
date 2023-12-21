using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Models;

public class InspectionCommentCreateModel
{
    [Required]
    [MaxLength(1000)]
    [MinLength(1)]
    public string Content { get; set; }
}
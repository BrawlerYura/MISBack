using System.ComponentModel.DataAnnotations;

namespace MISBack.Data.Models;

public class CommentCreateModel
{
    [Required]
    [MaxLength(1000)]
    [MinLength(1)]
    public string Content { get; set; }
    
    public Guid? ParentId { get; set; }
}
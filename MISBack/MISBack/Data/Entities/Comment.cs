namespace MISBack.Data.Entities;

public class Comment
{
    public Guid Id { get; set; }
    
    public DateTime CreateTime { get; set; }
    
    public DateTime? ModifiedDate { get; set; }
    
    public string Content { get; set; }
    
    public Guid AuthorId { get; set; }
    
    public Guid? ParentId { get; set; }
    
    public Guid ConsultationId { get; set; }
}
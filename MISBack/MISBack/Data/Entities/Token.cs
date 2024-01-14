namespace MISBack.Data.Entities;

public class Token
{
    public string InvalidToken { get; set; }
    
    public DateTime ExpiredDate { get; set; }
}
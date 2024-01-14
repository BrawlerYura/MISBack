using System.Net.Mail;

namespace MISBack.Data.Entities;

public class EmailingLogs
{
    public Guid Id { get; set; }
    public DateTime DateTime { get; set; }
    public string Logs { get; set; }
    
    public string Email { get; set; }
}
using MISBack.Data.Enums;

namespace MISBack.Data.Models;

public class IcdRootsReportRecordModel
{
    public string? PatientName { get; set; }
    public Guid PatientId { get; set; }
    public DateTime? PatientBirthdate { get; set; }
    public Gender? Gender { get; set; }
    public Dictionary<string, int>? VisitsByRoot { get; set; }
}
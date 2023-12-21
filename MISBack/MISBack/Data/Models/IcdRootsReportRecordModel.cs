using MISBack.Data.Enums;

namespace MISBack.Data.Models;

public class IcdRootsReportRecordModel
{
    public string? PatientName { get; set; }
    public DateTime? PatientBirthdate { get; set; }
    public Gender? Gender { get; set; }
    public List<int>? VisitsByRoot { get; set; }
}
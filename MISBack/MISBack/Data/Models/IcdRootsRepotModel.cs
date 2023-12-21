namespace MISBack.Data.Models;

public class IcdRootsRepotModel
{
    public IcdRootsRepotFiltersModel? Filters { get; set; }
    
    public List<IcdRootsReportRecordModel>? Records { get; set; }
    
    public List<int>? SummaryByRoot { get; set; }
}
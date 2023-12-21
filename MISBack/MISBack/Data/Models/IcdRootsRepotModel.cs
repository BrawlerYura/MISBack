namespace MISBack.Data.Models;

public class IcdRootsRepotModel
{
    public IcdRootsRepotFiltersModel? Filters { get; set; }
    
    public List<IcsRootsReportRecordModel>? Records { get; set; }
    
    public List<int>? SummaryByRoot { get; set; }
}
namespace MISBack.Data.Models;

public class PatientPagedListModel
{
    public List<PatientModel>? Patients { get; set; }
    public PageInfoModel? Pagination { get; set; }
}
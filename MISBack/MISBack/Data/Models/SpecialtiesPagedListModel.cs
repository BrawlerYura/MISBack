namespace MISBack.Data.Models;

public class SpecialtiesPagedListModel
{
    public List<SpecialityModel>? Specialities { get; set; }
    public PageInfoModel? Pagination { get; set; }
}
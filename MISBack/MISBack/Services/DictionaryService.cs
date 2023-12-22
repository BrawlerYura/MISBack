using MISBack.Data.Models;
using MISBack.Services.Interfaces;

namespace MISBack.Services;

public class DictionaryService : IDictionaryInterface
{
    public Task<SpecialtiesPagedListModel> GetSpecialtiesList(string? name, int page = 1, int size = 5)
    {
        throw new NotImplementedException();
    }

    public Task<Icd10SearchModel> GetDiagnosisList(string? request, int page = 1, int size = 5)
    {
        throw new NotImplementedException();
    }

    public Task<List<Icd10RecordModel>> GetRootsList()
    {
        throw new NotImplementedException();
    }
}
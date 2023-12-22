using Microsoft.AspNetCore.Mvc;
using MISBack.Data.Models;

namespace MISBack.Services.Interfaces;

public interface IDictionaryInterface
{
    Task<SpecialtiesPagedListModel> GetSpecialtiesList(string? name, int page = 1, int size = 5);

    Task<Icd10SearchModel> GetDiagnosisList(string? request, int page = 1, int size = 5);

    Task<List<Icd10RecordModel>> GetRootsList();
}
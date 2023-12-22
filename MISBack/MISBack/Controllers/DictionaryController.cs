using System.ComponentModel.Design;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using MISBack.Data.Models;

namespace MISBack.Controllers;

[ApiController]
[Route("api/dictionary")]
public class DictionaryController
{
    private readonly IDictionaryService _dictionaryService;

    public DictionaryController(IDictionaryService dictionaryService)
    {
        _dictionaryService = dictionaryService;
    }
    
    [HttpGet]
    [Route("speciality")]
    public async Task<SpecialtiesPagedListModel> GetSpecialtiesList([FromQuery] string? name, [FromQuery] int page = 1, [FromQuery] int size = 5)
    {
        return await _dictionaryService.GetSpecialtiesList(name, page, size);
    }
    
    [HttpGet]
    [Route("icd10")]
    public async Task<Icd10SearchModel> GetDiagnosisList([FromQuery] string? request, [FromQuery] int page = 1, [FromQuery] int size = 5)
    {
        return await _dictionaryService.GetDiagnosisList(request, page, size);
    }
    
    [HttpGet]
    [Route("icd10/roots")]
    public async Task<List<Icd10RecordModel>> GetRootsList()
    {
        return await _dictionaryService.GetRootsList();
    }
}
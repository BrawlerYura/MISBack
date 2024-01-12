using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MISBack.Data;
using MISBack.Data.Entities;
using MISBack.Data.Models;
using MISBack.Services.Interfaces;

namespace MISBack.Services;

public class DictionaryService : IDictionaryService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DictionaryService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<SpecialtiesPagedListModel> GetSpecialtiesList(string? name, int page = 1, int size = 5)
    {
        var query = _context.Speciality.AsQueryable();

        if (name != null & name != "")
        {
            query = query.Where(speciality => speciality.Name.Contains(name));
        }
        
        var count = (int)Math.Ceiling((double)query.Count() / (double)size);
        if (count == 0)
        {
            throw new BadHttpRequestException("no specialities were found with this request");
        }
        
        var specialities = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();
        
        var specialityListModel = CreateSpecialityModel(specialities);

        var specialtiesPagedListModel = new SpecialtiesPagedListModel
        {
            Specialities = specialityListModel,
            Pagination = new PageInfoModel
            {
                Size = specialityListModel.Count,
                Count = count,
                Current = page
            }
        };

        if (page < 1 || page > count)
        {
            throw new BadHttpRequestException("Invalid value for attribute page");
        }

        return specialtiesPagedListModel;
    }

    public async Task<Icd10SearchModel> GetDiagnosisList(string? request, int page = 1, int size = 5)
    {
        var query = _context.Icd10.AsQueryable();
        
        if(request != null & request != "")
        {
            query = query.Where(i => i.MkbName.Contains(request) || i.MkbCode.Contains(request));
        }
        
        var count = (int)Math.Ceiling((double)query.Count() / (double)size);
        if (count == 0)
        {
            throw new BadHttpRequestException("no specialities were found with this request");
        }
        
        var diagnoses = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        var diagnosisModelList = new List<Icd10RecordModel>();
        
        foreach (var diagnosis in diagnoses)
        {
            var diagnosisModel = new Icd10RecordModel
            {
                Id = diagnosis.Id,
                CreateTime = diagnosis.Date,
                Code = diagnosis.MkbCode,
                Name = diagnosis.MkbName
            };
            
            diagnosisModelList.Add(diagnosisModel);
        }

        var icd10SearchModel = new Icd10SearchModel
        {
            Records = diagnosisModelList,
            Pagination = new PageInfoModel
            {
                Size = diagnosisModelList.Count,
                Count = count,
                Current = page
            }
        };

        if (page < 1 || page > count)
        {
            throw new BadHttpRequestException("Invalid value for attribute page");
        }

        return icd10SearchModel;
    }

    public async Task<List<Icd10RecordModel>> GetRootsList()
    {
        var roots = await _context.Icd10.Where(i => i.IdParent == null).ToListAsync();
        if (roots == null)
        {
            throw new KeyNotFoundException("roots not found");
        }

        var icdModelList = new List<Icd10RecordModel>();

        foreach (var root in roots)
        {
            var icdModel = new Icd10RecordModel
            {
                Id = root.Id,
                CreateTime = root.Date,
                Code = root.MkbCode,
                Name = root.MkbName
            };
            icdModelList.Add(icdModel);
        }
        
        return icdModelList;
    }

    private List<SpecialityModel> CreateSpecialityModel(List<Speciality> specialities)
    {
        return _mapper.Map<List<SpecialityModel>>(specialities);
    }
}
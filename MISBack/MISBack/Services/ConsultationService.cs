using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MISBack.Data;
using MISBack.Data.Entities;
using MISBack.Data.Enums;
using MISBack.Data.Models;
using MISBack.Services.Interfaces;

namespace MISBack.Services;

public class ConsultationService : IConsultationService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ConsultationService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<InspectionPagedListModel> GetInspectionsList(bool grouped, List<string>? icdRoots, int page, int size)
    {
        var query = _context.Inspection.AsQueryable();

        var groupedInspections = query
            .Where(i => i.BaseInspectionId == null)
            .GroupBy(i => i.Id)
            .SelectMany(group => group.OrderBy(i => i.CreateTime)
                .Union(group.Join(query, i => i.Id, j => j.PreviousInspectionId, (i, j) => j)
                    .OrderBy(i => i.CreateTime)));

        var inspections = await groupedInspections.Skip((page - 1) * size)
            .Take(size).ToListAsync();

        var inspectionModels = new List<InspectionPreviewModel>();
        
        foreach (var inspection in inspections)
        {
            var inspectionModel = _mapper.Map<InspectionPreviewModel>(inspection);

            var diagnosisEntity = await _context.InspectionDiagnosis
                .Where(d => d.InspectionId == inspection.Id)
                .Join(
                    _context.Diagnosis,
                    id => id.DiagnosisId,
                    d => d.Id,
                    (id, d) => d
                )
                .Where(d => d.Type == DiagnosisType.Main)
                .FirstOrDefaultAsync();

            if (diagnosisEntity != null)
            {
                inspectionModel.Diagnosis = _mapper.Map<DiagnosisModel>(diagnosisEntity);
            }

            var hasNested = await _context.Inspection
                .AnyAsync(i => i.PreviousInspectionId == inspection.Id);

            inspectionModel.HasNested = hasNested;

            inspectionModel.HasChain = inspection.BaseInspectionId != null;
            
            inspectionModels.Add(inspectionModel);
        }
        
        var inspectionPagedListDto = new InspectionPagedListModel
        {
            Inspections = inspectionModels,
            Pagination = new PageInfoModel
            {
                Size = inspections.Count,
                Count = groupedInspections.Count(),
                Current = page
            }
        };

        return inspectionPagedListDto;
    }

    public async Task<ConsultationModel> GetConsultation(Guid consultationId)
    {
        var consultationEntity = await _context.Consultation.Where(c => c.Id == consultationId).FirstOrDefaultAsync();
        if (consultationEntity == null)
        {
            throw new KeyNotFoundException($"consultation with id {consultationId} not found");
        }

        var consultationModel = _mapper.Map<ConsultationModel>(consultationEntity);

        var specialityEntity = await _context.Speciality.FirstOrDefaultAsync(s => s.Id == consultationEntity.SpecialityId);
        if (specialityEntity == null)
        {
            throw new KeyNotFoundException($"speciality with id {consultationEntity.SpecialityId} not found");
        }

        consultationModel.Speciality = _mapper.Map<SpecialityModel>(specialityEntity);

        var commentsEntityList = await _context.Comment.Where(c => c.ConsultationId == consultationEntity.Id).ToListAsync();

        var commentModelList = new List<CommentModel>();

        foreach (var commentEntity in commentsEntityList)
        {
            var commentModel = _mapper.Map<CommentModel>(commentEntity);

            var author =
                await _context.Doctor.Where(d => d.Id == commentEntity.AuthorId).FirstOrDefaultAsync();
            if (author == null)
            {
                throw new KeyNotFoundException($"author with id {commentEntity.AuthorId} not found");
            }
            
            commentModel.Author = author.Name;
            
            commentModelList.Add(commentModel);
        }

        consultationModel.Comments = commentModelList;

        return consultationModel;
    }

    public async Task<Guid> AddComment(Guid consultationId, Guid authorId, CommentCreateModel comment)
    {
        var commentEntity = _mapper.Map<Comment>(comment);
        
        commentEntity.Id = Guid.NewGuid();
        commentEntity.CreateTime = DateTime.Now;
        commentEntity.AuthorId = authorId;
        commentEntity.ConsultationId = consultationId;

        await _context.Comment.AddAsync(commentEntity);

        return commentEntity.Id;
    }

    public async Task EditComment(Guid commentId, InspectionCommentCreateModel comment)
    {
        var commentEntity = await _context.Comment.Where(c => c.Id == commentId).FirstOrDefaultAsync();
        if (commentEntity == null)
        {
            throw new KeyNotFoundException($"comment with id {commentId} not found");
        }
        
        commentEntity.Content = comment.Content;
        commentEntity.ModifiedDate = DateTime.Now;

        await _context.SaveChangesAsync();
    }
}
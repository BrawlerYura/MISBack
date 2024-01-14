using AutoMapper;
using BlogApi.Exceptions;
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

    public async Task<InspectionPagedListModel> GetInspectionsList(List<string>? icdRoots, int page = 1, int size = 5,
        bool grouped = false)
    {
        var query = _context.Inspection.AsQueryable();

        if (grouped)
        {
            query = query.Where(i => i.PreviousInspectionId == null);
        }

        var inspectionsList = await query.ToListAsync();

        var inspectionModelList = new List<InspectionPreviewModel>();
        foreach (var inspection in inspectionsList)
        {
            var inspectionModel = _mapper.Map<InspectionPreviewModel>(inspection);

            var doctorEntity = await _context.Doctor.FirstOrDefaultAsync(d => d.Id == inspection.DoctorId);
            if (doctorEntity == null)
            {
                throw new KeyNotFoundException($"doctor with id {inspection.DoctorId} not found in inspection {inspection.Id}");
            }

            inspectionModel.Doctor = doctorEntity.Name;

            var patientEntity = await _context.Patient.FirstOrDefaultAsync(p => p.Id == inspection.PatientId);
            if (patientEntity == null)
            {
                throw new KeyNotFoundException($"patient with id {inspection.PatientId} not found in inspection {inspection.Id}");
            }

            inspectionModel.Patient = patientEntity.Name;

            var inspectionDiagnosisList = await _context.InspectionDiagnosis
                .Where(id => id.InspectionId == inspection.Id)
                .ToListAsync();

            foreach (var inspectionDiagnosis in inspectionDiagnosisList)
            {
                var diagnosis =
                    await _context.Diagnosis.FirstOrDefaultAsync(d => d.Id == inspectionDiagnosis.DiagnosisId);
                if (diagnosis != null & diagnosis.Type == DiagnosisType.Main)
                {
                    inspectionModel.Diagnosis = _mapper.Map<DiagnosisModel>(diagnosis);
                    break;
                }
            }

            if (icdRoots != null & icdRoots.Count > 0)
            {
                var isInRoot = await IsInRoot(inspectionModel.Diagnosis, icdRoots);
                if (!isInRoot)
                {
                    continue;
                }
            }

            inspectionModel.HasNested = inspection.BaseInspectionId == null;

            var nextInspection =
                await _context.Inspection.FirstOrDefaultAsync(i => i.PreviousInspectionId == inspection.Id);

            inspectionModel.HasChain = nextInspection != null;

            inspectionModelList.Add(inspectionModel);
        }

        var count = (int)Math.Ceiling((double)inspectionModelList.Count() / (double)size);

        if (size <= 0)
        {
            throw new BadHttpRequestException("Invalid value for attribute size");
        }

        inspectionModelList = inspectionModelList
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();

        var inspectionPagedListModel = new InspectionPagedListModel
        {
            Inspections = inspectionModelList,
            Pagination = new PageInfoModel
            {
                Size = size,
                Count = count,
                Current = page
            }
        };

        if (page < 1 || (page > count & count > 0))
        {
            throw new BadHttpRequestException("Invalid value for attribute page");
        }

        return inspectionPagedListModel;
    }

    private async Task<bool> IsInRoot(DiagnosisModel diagnosisModel, List<string>? icdRoots)
    {
        var diagnosis = await _context.Icd10.FirstOrDefaultAsync(i => i.MkbCode == diagnosisModel.Code);
        if (diagnosis == null)
        {
            throw new KeyNotFoundException($"diagnosis with code {diagnosisModel.Code} not found in icd-10");
        }

        var icdRoot = await _context.Icd10.FirstOrDefaultAsync(ir => ir.RecCode == diagnosis.RecCode.Substring(0, 2));

        return icdRoots.Contains(icdRoot.MkbCode.ToUpper()) || icdRoots.Contains(icdRoot.MkbName);
    }

    public async Task<ConsultationModel> GetConsultation(Guid consultationId)
    {
        var consultationEntity = await _context.Consultation.Where(c => c.Id == consultationId).FirstOrDefaultAsync();
        if (consultationEntity == null)
        {
            throw new KeyNotFoundException($"consultation with id {consultationId} not found");
        }

        var consultationModel = _mapper.Map<ConsultationModel>(consultationEntity);

        var specialityEntity =
            await _context.Speciality.FirstOrDefaultAsync(s => s.Id == consultationEntity.SpecialityId);
        if (specialityEntity == null)
        {
            throw new KeyNotFoundException($"speciality with id {consultationEntity.SpecialityId} not found");
        }

        consultationModel.Speciality = _mapper.Map<SpecialityModel>(specialityEntity);

        var commentsEntityList =
            await _context.Comment.Where(c => c.ConsultationId == consultationEntity.Id).ToListAsync();

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
        commentEntity.CreateTime = DateTime.UtcNow;
        commentEntity.AuthorId = authorId;
        commentEntity.ConsultationId = consultationId;

        if (comment.ParentId != null)
        {
            var parentCommentEntity = await _context.Comment.FirstOrDefaultAsync(c => c.Id == comment.ParentId);
            if (parentCommentEntity == null)
            {
                throw new BadHttpRequestException($"parent comment with id {comment.ParentId} not found");
            }

            if (parentCommentEntity.ConsultationId != commentEntity.ConsultationId)
            {
                throw new BadHttpRequestException(
                    "parent comment consultation does not match current comment consultation");
            }
        }

        await _context.Comment.AddAsync(commentEntity);
        await _context.SaveChangesAsync();
        return commentEntity.Id;
    }

    public async Task EditComment(Guid authorId, Guid commentId, InspectionCommentCreateModel comment)
    {
        var commentEntity = await _context.Comment.Where(c => c.Id == commentId).FirstOrDefaultAsync();
        if (commentEntity == null)
        {
            throw new KeyNotFoundException($"comment with id {commentId} not found");
        }
        if (commentEntity.Content == comment.Content)
        {
            throw new BadHttpRequestException("content has not been changed");
        }
        if (commentEntity.AuthorId != authorId)
        {
            throw new ForbiddenException("You can't change not yours comment");
        }

        commentEntity.Content = comment.Content;
        commentEntity.ModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}
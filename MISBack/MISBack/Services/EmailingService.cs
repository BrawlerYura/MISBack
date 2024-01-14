using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using MISBack.Data;
using MISBack.Data.Entities;
using Quartz;

namespace MISBack.Services.SchedulerService;

public class EmailingService : IJob
{
    private readonly ApplicationDbContext _context;

    public EmailingService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var query = _context.Inspection.Where(i => i.NextVisitDate != null).AsQueryable();

        query = query.Where(i => ((TimeSpan)(DateTime.UtcNow - i.NextVisitDate)).TotalMinutes > 60);

        var inspectionsWithPrevId = await _context.Inspection.Where(i => i.PreviousInspectionId != null)
            .Select(i => i.PreviousInspectionId).ToListAsync();

        var notVisitedInspections = await query.Where(i => !inspectionsWithPrevId.Contains(i.Id)).ToListAsync();

        var patientsIds = new HashSet<Guid>();
        var patientsEmails = new List<string>();
        
        foreach (var notVisitedInspection in notVisitedInspections)
        {
            if (patientsIds.Add(notVisitedInspection.PatientId))
            {
                var patientEntity = await _context.Patient.FirstOrDefaultAsync(p => p.Id == notVisitedInspection.PatientId);
                if(patientEntity != null)
                {
                    patientsEmails.Add(patientEntity.Email);
                }
            }
        }

        
        using var client = new SmtpClient("smtp.gmail.com");
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential("sitdikov.yuriy@gmail.com", "stbq udtb nwjn hanl");
        client.Port = 587;
        client.EnableSsl = true;

        var mailMessage = new MailMessage
        {
            From = new MailAddress("sitdikov.yuriy@gmail.com"),
            Subject = "Inspection",
            Body = "Дарова, ты тут это, на осмотр записан, но не пришел. Не дело это!" +
                   "Свяжись со своим врачом и утверди другую дату для осмотра. Пожалуйста 🥺" +
                   "Иначе такие сообщения тебе будут приходить каждый день 😊, и мы от тебя не отстанем! 💀" +
                   "Береги себя и своих близких! 🤗",
            IsBodyHtml = true,
        };

        foreach (var email in patientsEmails)
        {
            try
            {
                mailMessage.To.Add(email);

                client.Send(mailMessage);
            }
            catch (SmtpException ex)
            {
                await _context.EmailingLogs.AddAsync(new EmailingLogs
                {
                    Id = Guid.NewGuid(),
                    DateTime = DateTime.UtcNow,
                    Logs = ex.Message,
                    Email = email
                });

                await _context.SaveChangesAsync();
            }
        }
    }
}
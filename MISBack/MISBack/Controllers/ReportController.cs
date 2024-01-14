using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using MISBack.Data.Models;
using MISBack.Services.Interfaces;

namespace MISBack.Controllers;

[ApiController]
[Route("api/report/icdrootsreport")]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet]
    public async Task<IcdRootsReportModel> GetReport([FromQuery] DateTime start, [FromQuery] DateTime end,
        [FromQuery] List<string>? icdRoots)
    {
        return await _reportService.GetReport(start, end, icdRoots);
    }

    [HttpPost]
    public async Task SendEmail()
    {
        try
        {
            var fromAddress = new MailAddress("sitdikov.yuriy@gmail.com", "stbq udtb nwjn hanl");
            var toAddress = new MailAddress("sitdikov.yuriy2@gmail.com", "yurchik");
            const string fromPassword = "stbq udtb nwjn hanl";
            const string subject = "Subject";
            const string body = "Body";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using var message = new MailMessage(fromAddress, toAddress);
            message.Subject = subject;
            message.Body = body;
            smtp.Send(message);
        }
        catch (SmtpException ex)
        {
            Console.WriteLine(ex);
        }
    }
}
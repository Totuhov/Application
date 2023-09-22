
namespace Application.Services;

using Microsoft.Extensions.Configuration;

using System.Net;
using System.Net.Mail;
using System;

using Application.Services.Interfaces;

public class MessageService : IMessageService
{
    private readonly IConfiguration _configuration;

    public MessageService(IConfiguration configuration)
    {
        this._configuration = configuration;
    }

    public bool SendEmail(string recieverEmail, string senderName, string senderEmail, string text)
    {
        try
        {
            MailAddress to = new(recieverEmail);
            MailAddress from = new(senderEmail);

            MailMessage email = new(from, to)
            {
                Subject = $"{senderName} send you a new message via Portfolio.com",
                Body = text
            };

            string host = _configuration["MailHostName"];
            string username = _configuration["MailServerUsername"];
            string password = _configuration["MailServerPassword"];

            SmtpClient smtp = new()
            {
                Host = host,
                Port = int.Parse(_configuration["Port"]),
                Credentials = new NetworkCredential(username, password),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true
            };

            smtp.Send(email);

            return true;
        }
        catch (Exception)
        {

            return false;
        }


    }

    public bool SendVerificationEmail(string reciever, string subject, string confitmLink)
    {
        try
        {
            string host = _configuration["MailHostName"];
            string username = _configuration["MailServerUsername"];
            string password = _configuration["MailServerPassword"];

            MailAddress to = new(reciever);
            MailAddress from = new("noreplay@portfolio.confirm");

            MailMessage email = new(from, to)
            {
                Subject = subject,
                IsBodyHtml = true,
                Body = confitmLink
            };

            SmtpClient smtp = new()
            {
                Host = host,
                Port = 587,
                Credentials = new NetworkCredential(username, password),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true
            };

            smtp.Send(email);

            return true;
        }
        catch (Exception)
        {

            return false;
        }
    }
}

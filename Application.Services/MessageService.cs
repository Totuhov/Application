
namespace Application.Services;

using System.Net;
using System.Net.Mail;
using System;

using Application.Services.Interfaces;

public class MessageService : IMessageService
{
    public void SendEmail(string recieverEmail, string senderName, string senderEmail, string text)
    {

        MailAddress to = new(recieverEmail);
        MailAddress from = new(senderEmail);

        MailMessage email = new(from, to)
        {
            Subject = $"{senderName} send you a new message via Portfolio.com",
            Body = text
        };

        SmtpClient smtp = new()
        {
            Host = "smtp-relay.brevo.com",
            Port = 587,
            Credentials = new NetworkCredential("nikolaytotuhov@gmail.com", "LCrJMFvS2aAPR34q"),
            DeliveryMethod = SmtpDeliveryMethod.Network,
            EnableSsl = true
        };

        smtp.Send(email);
    }

    public bool SendVerificationEmail(string reciever, string subject, string confitmLink)
    {
        try
        {
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
                Host = "smtp-relay.brevo.com",
                Port = 587,
                Credentials = new NetworkCredential("nikolaytotuhov@gmail.com", "LCrJMFvS2aAPR34q"),
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

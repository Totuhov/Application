
namespace Application.Services;

using System.Net;
using System.Net.Mail;
using System;
using System.IO;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;

using Application.Services.Interfaces;

public class MessageService : IMessageService
{
    public void SendEmail(string recieverEmail,string senderName, string senderEmail, string text)
    {

        MailAddress to = new(recieverEmail);
        MailAddress from = new(senderEmail);

        MailMessage email = new(from, to)
        {
            Subject = $"{senderName} send you a new message via Portfolio.com",
            Body = text
        };

        System.Net.Mail.SmtpClient smtp = new()
        {
            Host = "smtp.mailgun.org",
            Port = 587,
            Credentials = new NetworkCredential("postmaster@sandboxfd890ce84ecb4f5f9b26cc13db3578b4.mailgun.org", "a205d3cc2e1789a5f0318f7d44493925-6d8d428c-fe8368de"),
            DeliveryMethod = SmtpDeliveryMethod.Network,
            EnableSsl = true
        };

        try
        {
            smtp.Send(email);
        }
        catch (SmtpException ex)
        {
            Console.WriteLine(ex.ToString());
        }      
    }
    
}

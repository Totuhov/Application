
namespace Application.Services;

using System.Net;
using System.Net.Mail;

using Application.Services.Interfaces;

public class MessageService : IMessageService
{
    public void SendEmail(string recieverEmail,string senderName, string senderEmail, string text)
    {

        MailAddress to = new MailAddress(recieverEmail);
        MailAddress from = new MailAddress(senderEmail);

        MailMessage email = new MailMessage(from, to);
        email.Subject = $"{senderName} send you a new message via Portfolio.com";
        email.Body = text;

        SmtpClient smtp = new SmtpClient();
        smtp.Host = "smtp.mailgun.org";
        smtp.Port = 587;
        smtp.Credentials = new NetworkCredential("postmaster@sandboxfd890ce84ecb4f5f9b26cc13db3578b4.mailgun.org", "a205d3cc2e1789a5f0318f7d44493925-6d8d428c-fe8368de");
        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtp.EnableSsl = true;

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

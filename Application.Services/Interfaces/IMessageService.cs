
namespace Application.Services.Interfaces;

public interface IMessageService
{
    bool SendEmail(string recieverEmail, string senderName, string senderEmail, string text);
    bool SendVerificationEmail(string reciever, string subject, string confitmLink);
}

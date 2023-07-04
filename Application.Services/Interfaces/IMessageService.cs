
namespace Application.Services.Interfaces;

public interface IMessageService
{
    void SendEmail(string recieverEmail, string senderName, string senderEmail, string text);
    void SendMessageSmtp();
}

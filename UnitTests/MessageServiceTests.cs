
using Application.Services;
using Application.Services.Interfaces;
using NUnit.Framework;

namespace Application.UnitTests;

[TestFixture]
public class MessageServiceTests
{
    private readonly string _validRecieverEmail = "test@valid-reciever.com";
    private readonly string _invalidRecieverEmail = "test1valid-reciever";
    private readonly string _senderName = "Sender";
    private readonly string _validSenderEmail = "test@valid-sender.com";
    private readonly string _invalidSenderEmail = "test1valid-sender";
    private readonly string _text = "Random email text";

    [Test]
    public void Test_SendEmail_Succeed()
    {
        IMessageService service = new MessageService();

        bool result = service.SendEmail(_validRecieverEmail, _senderName, _validSenderEmail, _text);

        Assert.That(result, Is.True);
    }

    [Test]
    public void Test_SendEmail_Fail_InvalidRecieverEmail()
    {
        IMessageService service = new MessageService();

        bool result = service.SendEmail(_invalidRecieverEmail, _senderName, _validSenderEmail, _text);

        Assert.That(result, Is.False);
    }
    [Test]
    public void Test_SendEmail_Fail_InvalidSenderEmail()
    {
        IMessageService service = new MessageService();

        bool result = service.SendEmail(_validRecieverEmail, _senderName, _invalidSenderEmail, _text);

        Assert.That(result, Is.False);
    }

    [Test]
    public void Test_SendVerificationEmail_Succeed()
    {
        IMessageService service = new MessageService();

        bool result = service.SendVerificationEmail(_validRecieverEmail, _text, _text);

        Assert.That(result, Is.True);
    }

    [Test]
    public void Test_SendVerificationEmail_Fail()
    {
        IMessageService service = new MessageService();

        bool result = service.SendVerificationEmail(_invalidRecieverEmail, _text, _text);

        Assert.That(result, Is.False);
    }
}

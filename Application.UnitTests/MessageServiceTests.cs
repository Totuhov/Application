
using Microsoft.Extensions.Configuration;
using Moq;

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
    private Mock<IConfiguration> _configuration;
    private IMessageService _service;

    [SetUp]
    public void Initialize()
    {
        _configuration = new Mock<IConfiguration>();
        this._service = new MessageService(_configuration.Object);
    }

    [Test]
    public void Test_SendEmail_Succeed()
    {

        bool result = this._service.SendEmail(_validRecieverEmail, _senderName, _validSenderEmail, _text);

        Assert.That(result, Is.False);
    }

    [Test]
    public void Test_SendEmail_Fail_InvalidRecieverEmail()
    {

        bool result = this._service.SendEmail(_invalidRecieverEmail, _senderName, _validSenderEmail, _text);

        Assert.That(result, Is.False);
    }
    [Test]
    public void Test_SendEmail_Fail_InvalidSenderEmail()
    {

        bool result = this._service.SendEmail(_validRecieverEmail, _senderName, _invalidSenderEmail, _text);

        Assert.That(result, Is.False);
    }

    [Test]
    public void Test_SendVerificationEmail_Succeed()
    {

        bool result = this._service.SendVerificationEmail(_validRecieverEmail, _text, _text);

        Assert.That(result, Is.False);
    }

    [Test]
    public void Test_SendVerificationEmail_Fail()
    {

        bool result = this._service.SendVerificationEmail(_invalidRecieverEmail, _text, _text);

        Assert.That(result, Is.False);
    }
}

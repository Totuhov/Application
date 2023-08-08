
using Microsoft.Extensions.Caching.Memory;

using static Application.Common.GlobalConstants;

namespace Application.IntegrationTests;

public class PortfolioControllerTests
{
    private Mock<IPortfolioService> mockPortfolioService;
    private Mock<IUserService> mockUserService;
    private Mock<IMessageService> mockMessageService;
    private Mock<IMemoryCache> mockMemoryCache;
    private MockControllerContext mockControllerContext;
    private PortfolioController controller;
    private ClaimsPrincipal user;

    [SetUp]
    public void Initialize()
    {
        this.mockPortfolioService = new Mock<IPortfolioService>();
        this.mockUserService = new Mock<IUserService>();
        this.mockMessageService = new Mock<IMessageService>();
        this.mockMemoryCache = new Mock<IMemoryCache>();
        this.mockMemoryCache = new Mock<IMemoryCache>();
        this.controller = new PortfolioController(
            mockPortfolioService.Object,
            mockUserService.Object,
            mockMessageService.Object,
            mockMemoryCache.Object);

        this.user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "guest"),
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "Admin")
        }, "mock"));

        mockControllerContext = new MockControllerContext(this.user);

        this.controller.ControllerContext = mockControllerContext;
        controller.TempData = new TempDataDictionary(mockControllerContext.HttpContext, Mock.Of<ITempDataProvider>());
    }

    [Test]
    public async Task Index_UserHasNotPortfolio()
    {
        this.mockPortfolioService.Setup(service => service.LogedInUserHasPortfolio("1")).ReturnsAsync(false);

        var result = await controller.Index() as ViewResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Model, Is.Not.Null);
    }

    [Test]
    public async Task Index_UserHasPortfolio()
    {
        this.mockPortfolioService.Setup(service => service.LogedInUserHasPortfolio("1")).ReturnsAsync(true);

        var result = await controller.Index() as RedirectToActionResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.ActionName, Is.EqualTo("Details"));
    }

    [Test]
    public async Task Index_TrowsExeption()
    {
        mockPortfolioService.Setup(s => s.LogedInUserHasPortfolio("1")).Throws<Exception>();

        var result = await controller.Index() as RedirectToActionResult;

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(result.ControllerName, Is.EqualTo("Home"));
        });
    }

    [Test]
    public async Task Details_UserHasNotPortfolio()
    {
        var model = new PortfolioViewModel();

        this.mockUserService.Setup(service => service.IsUserExists("1")).ReturnsAsync(true);
        this.mockPortfolioService.Setup(service => service.GetPortfolioFromRouteAsync("1"))
            .ReturnsAsync(model);

        var memoryCache = Mock.Of<IMemoryCache>();
        var cachEntry = Mock.Of<ICacheEntry>();

        var mockMemoryCache = Mock.Get(memoryCache);
        mockMemoryCache
            .Setup(m => m.CreateEntry(It.IsAny<object>()))
            .Returns(cachEntry);

        var cachedResponse = memoryCache.Set(PortfolioCacheKey, model);

        var result = await controller.Details("1");

        Assert.IsNotNull(cachedResponse);

    }

    [Test]
    public async Task Details_UserHasPortfolio()
    {
        string validId = "1";

        // Create a sample PortfolioViewModel for testing
        var cacheModel = new PortfolioViewModel 
        { 
            Id = validId,
            UserName= "admin",
            Email = "test@email.net",
            ProfileImage = new()
            {
                ImageId = "113",
                ApplicationUserId = null,
                ContentType = "application/json",
            },
        };

        this.mockUserService.Setup(service => service.IsUserExists(validId)).ReturnsAsync(true);

        this.mockMemoryCache
            .Setup(c => c.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny))
            .Returns(false);

        this.mockPortfolioService
            .Setup(s => s.GetPortfolioFromRouteAsync(validId))
            .ReturnsAsync(cacheModel);

        // Act
        var result = await controller.Details(validId);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<NotFoundResult>(result);

    }


    [Test]
    public async Task Details_InvalidUrl()
    {
        this.mockUserService.Setup(service => service.IsUserExists("1")).ReturnsAsync(false);

        var result = await controller.Details("1");

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task Details_ThrowsException()
    {
        this.mockUserService.Setup(service => service.IsUserExists("1")).Throws<Exception>();

        var result = await controller.Details("1") as RedirectToActionResult;

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(result.ControllerName, Is.EqualTo("Home"));
        });
    }

    [Test]
    public async Task Create_ValidId()
    {
        this.mockPortfolioService.Setup(service => service.LogedInUserHasPortfolio("1"))
            .ReturnsAsync(false);

        var result = await controller.Create("guest") as RedirectToActionResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.ActionName, Is.EqualTo("Details"));
    }

    [Test]
    public async Task Create_ThrowsException()
    {
        this.mockPortfolioService.Setup(service => service.LogedInUserHasPortfolio("1"))
            .ReturnsAsync(false);
        this.mockPortfolioService.Setup(service => service.CreateFirstPortfolioAsync("1"))
            .Throws<Exception>();

        var result = await controller.Create("guest") as RedirectToActionResult;

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(result.ControllerName, Is.EqualTo("Home"));
        });
    }

    [Test]
    public async Task EditDescription_ValidId()
    {
        this.mockPortfolioService.Setup(service => service.GetEditDescriptionViewModelAsync("1"))
            .ReturnsAsync(new EditDescriptionPortfolioViewModel());

        var result = await controller.EditDescription("guest") as ViewResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Model, Is.Not.Null);
    }

    [Test]
    public async Task EditDescription_InvalidId()
    {
        var result = await controller.EditDescription("1");

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task EditDescription_ThrowsException()
    {
        this.mockPortfolioService.Setup(service => service.GetEditDescriptionViewModelAsync("1"))
            .Throws<Exception>();

        var result = await controller.EditDescription("guest") as RedirectToActionResult;

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(result.ControllerName, Is.EqualTo("Home"));
        });
    }

    [Test]
    public async Task EditDescription_Post_WithValidModel()
    {
        EditDescriptionPortfolioViewModel model = new();

        var result = await controller.EditDescription(model) as RedirectToActionResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.ActionName, Is.EqualTo("Details"));
    }

    [Test]
    public async Task EditDescription_Post_WithInvalidModel()
    {
        EditDescriptionPortfolioViewModel model = new();
        controller.ModelState.AddModelError("Title", "1");
        var result = await controller.EditDescription(model) as ViewResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Model, Is.Not.Null);
    }

    [Test]
    public async Task EditDescription_Post_ThrowsError()
    {
        EditDescriptionPortfolioViewModel model = new();
        this.mockPortfolioService.Setup(service => service.SaveDescriptionAsync(model, "1"))
            .Throws<Exception>();

        var result = await controller.EditDescription(model) as RedirectToActionResult;

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(result.ControllerName, Is.EqualTo("Home"));
        });
    }
    [Test]
    public async Task EditAbout_ValidId()
    {
        this.mockPortfolioService.Setup(service => service.GetEditAboutViewModelAsync("1"))
            .ReturnsAsync(new EditAboutPortfolioViewModel());

        var result = await controller.EditAbout("guest") as ViewResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Model, Is.Not.Null);
    }

    [Test]
    public async Task EditAbout_InvalidId()
    {
        var result = await controller.EditAbout("1");

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task EditAbout_ThrowsException()
    {
        this.mockPortfolioService.Setup(service => service.GetEditAboutViewModelAsync("1"))
            .Throws<Exception>();

        var result = await controller.EditAbout("guest") as RedirectToActionResult;

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(result.ControllerName, Is.EqualTo("Home"));
        });
    }

    [Test]
    public async Task EditAbout_Post_WithValidModel()
    {
        EditAboutPortfolioViewModel model = new();

        var result = await controller.EditAbout(model) as RedirectToActionResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.ActionName, Is.EqualTo("Details"));
    }

    [Test]
    public async Task EditAbout_Post_WithInvalidModel()
    {
        EditAboutPortfolioViewModel model = new();
        controller.ModelState.AddModelError("Title", "1");
        var result = await controller.EditAbout(model) as ViewResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Model, Is.Not.Null);
    }

    [Test]
    public async Task EditAbout_Post_ThrowsError()
    {
        EditAboutPortfolioViewModel model = new();
        this.mockPortfolioService.Setup(service => service.SaveAboutAsync(model, "1"))
            .Throws<Exception>();

        var result = await controller.EditAbout(model) as RedirectToActionResult;

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(result.ControllerName, Is.EqualTo("Home"));
        });
    }

    [Test]
    public async Task SendEmail_WithValidId()
    {
        this.mockUserService.Setup(service => service.IsUserExists("1"))
            .ReturnsAsync(true);

        var result = await controller.SendEmail("1") as ViewResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Model, Is.Not.Null);
    }

    [Test]
    public async Task SendEmail_WithInvalidId()
    {
        EditAboutPortfolioViewModel model = new();
        this.mockUserService.Setup(service => service.IsUserExists("1"))
            .ReturnsAsync(false);

        var result = await controller.SendEmail("1");

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task SendEmail_ThrowsException()
    {
        EditAboutPortfolioViewModel model = new();
        this.mockUserService.Setup(service => service.IsUserExists("1"))
            .Throws<Exception>();

        var result = await controller.SendEmail("1") as RedirectToActionResult;

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(result.ControllerName, Is.EqualTo("Home"));
        });
    }

    [Test]
    public void SendEmail_Post_WithtInvalidId()
    {
        ContactFormViewModel model = new();
        controller.ModelState.AddModelError("Title", "1");

        var result = controller.SendEmail(model, "1") as ViewResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Model, Is.Not.Null);
    }

    [Test]
    public void SendEmail_Post_ThrowsException()
    {
        ContactFormViewModel model = new();

        var result = controller.SendEmail(model, "1") as RedirectToActionResult;

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.ActionName, Is.EqualTo("GeneralError"));
        });
    }

    [Test]
    public void SendEmail_ModelIsValid_Success()
    {
        var model = new ContactFormViewModel
        {
            SenderName = "John Doe",
            SenderEmail = "john@example.com",
            Text = "Hello, this is a test email!",
            RecieverEmail = "receiver@example.com"
        };
        string id = "some_id";

        controller.ControllerContext = CreateControllerContextWithFormData(model);

        var result = controller.SendEmail(model, id) as RedirectToActionResult;

        mockMessageService.Verify(m => m.SendEmail(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>()
        ), Times.Once);

        Assert.That(result.ActionName, Is.EqualTo("Details"));
        Assert.That(result?.RouteValues?["id"], Is.EqualTo(id));
    }

    /// <summary>
    /// Special for the last test (needed new ControllerContext)
    /// </summary>
    /// <param name="formData"></param>
    /// <returns></returns>
    private static ControllerContext CreateControllerContextWithFormData(ContactFormViewModel formData)
    {
        var formCollection = new FormCollection(new Dictionary<string, StringValues>
        {
            { "senderName", formData.SenderName },
            { "senderEmail", formData.SenderEmail },
            { "text", formData.Text },
            { "recieverEmail", formData.RecieverEmail }
        });

        var httpContext = new DefaultHttpContext { Request = { Form = formCollection } };

        return new ControllerContext()
        {
            HttpContext = httpContext
        };
    }
}

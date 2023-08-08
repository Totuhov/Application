using Application.Data.Models;

namespace Application.UnitTests;

[TestFixture]
public class PortfolioServiceTests
{
    private ApplicationDbContext _context;
    private IPortfolioService _portfolioService;

    [OneTimeSetUp]
    public void TestInitialize()
    {
        List<ApplicationUser> users = new()
        {
            new ApplicationUser()
            {
                Id = "1",
                UserName = "guest",
                Portfolio = new()
                {
                    Id = "133",
                    GreetingsMessage = "Greetings message",
                    Description = "Description",
                    UserDisplayName = "User's display name",
                    About = "About user",
                    ImageId = "111",
                    ApplicationUserId = "1"                    
                },
                SocialMedia = new()
            },
            new ApplicationUser()
            {
                Id = "3",
                UserName = "vv",
                Portfolio = new()
                {
                    Id = "134",
                    GreetingsMessage = "Greetings message",
                    Description = "Description",
                    UserDisplayName = "User's display name",
                    About = "About user",
                    ImageId = "113",
                    ApplicationUserId = "3"
                },
                SocialMedia = new()
            },
            new ApplicationUser()
            {
                Id = "2",
                UserName = "Test"
            }
        };

        List<Image> images = new()
        {
            new()
            {
                ImageId = "111",
                ApplicationUserId = null,
                FileExtension = ".png",
                Bytes = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68,
                    82, 0, 0, 2, 0, 0, 0, 2, 0, 8, 2, 0, 0, 0, 123, 26, 67, 173, 0, 0, 0 },
                Characteristic = "defaultProfileImage"
            },
            new()
            {
                ImageId = "112",
                ApplicationUserId = null,
                FileExtension = ".png",
                Bytes = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68,
                    82, 0, 0, 2, 0, 0, 0, 2, 0, 8, 2, 0, 0, 0, 123, 26, 67, 173, 0, 0, 0 },
                Characteristic = "defaultProjectImage"
            },
            new()
            {
                ImageId = "113",
                ApplicationUserId = null,
                FileExtension = ".png",
                Bytes = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68,
                    82, 0, 0, 2, 0, 0, 0, 2, 0, 8, 2, 0, 0, 0, 123, 26, 67, 173, 0, 0, 0 }
            }
        };

        List<Project> projects = new()
        {
            new()
            {
                Id = "771",
                Name = "Test project",
                ImageId = "112",
                Description = "Project description",
                Url = null,
                ApplicationUserId = "1",
            }
        };

        Article article = new()
        {
            Id = "221",
            ApplicationUserId = "1",
            Title = "Title",
            Content = "Mir wurde gesagt von denen, dass die die Ruckmeldkarte Ihnen schiken " +
            "werden. Wahrscheinlich haben sie das noch nich gemacht. Ich habe mit denen " +
            "gesprochen und sie werden, dass sie Ihnen so schnell wie möglich zusenden werden.",
            CreatedOn = DateTime.Now,
            EditedOn = DateTime.Now,
            IsDeleted = false,
        };

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryPortfolioServiceDatabase")
            .Options;

        this._context = new ApplicationDbContext(options);
        this._context.Users.AddRange(users);
        this._context.Projects.AddRange(projects);
        this._context.Images.AddRange(images);
        this._context.Articles.Add(article);
        this._context.SaveChanges();

        this._portfolioService = new PortfolioService(this._context);
    }

    [Test]
    public async Task Test_CreateFirstPortfolioAsync_Succeed()
    {
        string userId = "2"; // This user still have not Portfolio

        ApplicationUser? user = await _context.Users.FindAsync(userId);

        await this._portfolioService.CreateFirstPortfolioAsync(userId);
        Assert.Multiple(() =>
        {
            Assert.That(user?.Portfolio, Is.Not.EqualTo(null));
            Assert.That(user?.Portfolio?.Image, Is.Not.EqualTo(null));
        });
    }

    [Test]
    [Order(2)]
    public async Task Test_GetEditDescriptionViewModelAsync_Succeed()
    {
        string userId = "1";
        IPortfolioService service = new PortfolioService(_context);

        EditDescriptionPortfolioViewModel? model = await this._portfolioService
            .GetEditDescriptionViewModelAsync(userId);

        Assert.Multiple(() =>
        {
            Assert.That(model, Is.Not.Null);
            Assert.That(model?.GreetingsMessage, Is.EqualTo("Greetings message"));
            Assert.That(model?.Description, Is.EqualTo("Description"));
            Assert.That(model?.UserDisplayName, Is.EqualTo("User's display name"));
        });
    }

    [Test]
    public async Task Test_GetEditAboutViewModelAsync_Succeed()
    {
        string userId = "1";

        EditAboutPortfolioViewModel? model = await this._portfolioService.GetEditAboutViewModelAsync(userId);

        Assert.Multiple(() =>
        {
            Assert.That(model, Is.Not.Null);
            Assert.That(model?.About, Is.EqualTo("About user"));
        });
    }

    [Test]
    [Order(1)]
    public async Task Test_GetPortfolioFromRouteAsync_Succeed()
    {
        string username = "guest";

        PortfolioViewModel? testModel = await this._portfolioService.GetPortfolioFromRouteAsync(username);

        Assert.That(testModel, Is.Not.EqualTo(null));
        Assert.Multiple(() =>
        {
            Assert.That(testModel.About, Is.EqualTo("About user"));
            Assert.That(testModel.ProfileImage.ImageId, Is.EqualTo("111")); // There was no profil image
        });
    }

    [Test]
    public async Task Test_LogedInUserHasPortfolio_Succeed()
    {
        string userId = "1";

        bool testResult = await this._portfolioService.LogedInUserHasPortfolio(userId);

        Assert.That(testResult, Is.EqualTo(true));
    }

    [Test]
    [Order(3)]
    public async Task Test_LogedInUserHasPortfolio_Fail()
    {
        string userId = "2"; // user with id 2 has not portfolio

        bool testResult = await this._portfolioService.LogedInUserHasPortfolio(userId);

        Assert.That(testResult, Is.EqualTo(false));
    }

    [Test]
    public async Task Test_SaveDescriptionAsync_Succeed()
    {
        string userId = "1";
        string userName = "guest";
        EditDescriptionPortfolioViewModel model = 
            await this._portfolioService.GetEditDescriptionViewModelAsync(userId);

        model.Description = "new description";
        await this._portfolioService.SaveDescriptionAsync(model, userId);

        PortfolioViewModel portfolioModel = await this._portfolioService.GetPortfolioFromRouteAsync(userName);

        Assert.That(portfolioModel, Is.Not.Null);
        Assert.That(portfolioModel.Description, Is.EqualTo("new description"));
    }

    [Test]
    public async Task Test_SaveAboutAsync_Succeed()
    {
        string userId = "1";
        string userName = "guest";
        EditAboutPortfolioViewModel? model =
            await this._portfolioService.GetEditAboutViewModelAsync(userId);
        model.About = "new about";

        await this._portfolioService.SaveAboutAsync(model, userId);

        PortfolioViewModel? portfolioModel = await this._portfolioService.GetPortfolioFromRouteAsync(userName);

        Assert.That(portfolioModel, Is.Not.Null);
        Assert.That(portfolioModel.About, Is.EqualTo("new about"));
    }

    [Test]
    public async Task Test_GetAllUsersByRegexAsync_SucceedWithResult()
    {
        string expression = "v";

        List<PreviewPortfolioViewModel> ports = await this._portfolioService.GetAllUsersByRegexAsync(expression);

        Assert.That(ports, Is.Not.Empty);
    }

    [Test]
    public async Task Test_GetAllUsersByRegexAsync_SucceedWithoutResult()
    {
        string expression = "ww";

        List<PreviewPortfolioViewModel> ports = await this._portfolioService.GetAllUsersByRegexAsync(expression);

        Assert.That(ports, Is.Empty);
    }
}
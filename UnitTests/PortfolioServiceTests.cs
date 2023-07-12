
using Application.Data;
using Application.Data.Models;
using Application.Services.Interfaces;
using Application.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Application.Web.ViewModels.Portfolio;

namespace Application.UnitTests;

[TestFixture]
public class PortfolioServiceTests
{
    private ApplicationDbContext _context;

    [OneTimeSetUp]
    public void TestInitialize()
    {
        List<ApplicationUser> users = new()
        {
            new ApplicationUser()
            {
                Id = "55",
                UserName = "guest",
                Portfolio = new()
                {
                    Id = "133",
                    GreetingsMessage = "Greetings message",
                    Description = "Description",
                    UserDisplayName = "User's display name",
                    About = "About user",
                    ApplicationUserId = "55"                       
                }
            },
            new ApplicationUser()
            {
                Id = "57",
                UserName = "vv",
                Portfolio = new()
                {
                    Id = "134",
                    GreetingsMessage = "Greetings message",
                    Description = "Description",
                    UserDisplayName = "User's display name",
                    About = "About user",
                    ImageId = "113",
                    ApplicationUserId = "57"
                }
            },
            new ApplicationUser()
            {
                Id = "56",
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
                ApplicationUserId = "55",
            }
        };


        Article article = new()
        {
            Id = "221",
            ApplicationUserId = "55",
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
    }

    [Test]
    public async Task Test_CreateFirstPortfolioAsync_Succeed()
    {
        string userId = "56"; // This user still have not Portfolio
        IPortfolioService service = new PortfolioService(_context);

        ApplicationUser? user = await _context.Users.FindAsync(userId);

        await service.CreateFirstPortfolioAsync(userId);
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
        string userId = "55";
        IPortfolioService service = new PortfolioService(_context);

        EditDescriptionPortfolioViewModelViewModel? model = await service.GetEditDescriptionViewModelAsync(userId);

        Assert.Multiple(() =>
        {
            Assert.That(model, Is.Not.Null);
            Assert.That(model?.GreetingsMessage, Is.EqualTo("Greetings message"));
            Assert.That(model?.Description, Is.EqualTo("Description"));
            Assert.That(model?.UserDisplayName, Is.EqualTo("User's display name"));
        });
    }
    [Test]
    public async Task Test_GetEditDescriptionViewModelAsync_Fail()
    {
        string userId = "3"; // there is no user with Id == "3"
        IPortfolioService service = new PortfolioService(_context);

        EditDescriptionPortfolioViewModelViewModel? model = await service.GetEditDescriptionViewModelAsync(userId);

        Assert.That(model, Is.Null);
    }

    [Test]
    public async Task Test_GetEditAboutViewModelAsync_Succeed()
    {
        string userId = "55";
        IPortfolioService service = new PortfolioService(_context);

        EditAboutPortfolioViewModelViewModel? model = await service.GetEditAboutViewModelAsync(userId);

        Assert.Multiple(() =>
        {
            Assert.That(model, Is.Not.Null);
            Assert.That(model?.About, Is.EqualTo("About user"));
        });
    }
    [Test]
    public async Task Test_GetEditAboutViewModelAsync_Fail()
    {
        string userId = "3"; // there is no user with Id == "3"
        IPortfolioService service = new PortfolioService(_context);

        EditAboutPortfolioViewModelViewModel? model = await service.GetEditAboutViewModelAsync(userId);

        Assert.That(model, Is.Null);
    }
    [Test]
    [Order(1)]
    public async Task Test_GetPortfolioFromRouteAsync_Succeed()
    {
        string username = "guest";
        IPortfolioService service = new PortfolioService(_context);

        PortfolioViewModel? testModel = await service.GetPortfolioFromRouteAsync(username);

        Assert.That(testModel, Is.Not.EqualTo(null));
        Assert.Multiple(() =>
        {
            Assert.That(testModel.About, Is.EqualTo("About user"));
            Assert.That(testModel.ProfileImage.ImageId, Is.EqualTo("111")); // There was no profil image
        });
    }

    [Test]
    public async Task Test_GetPortfolioFromRouteAsync_FailWithoutUser()
    {
        string username = "gues"; // there is no user with username == "gues"
        IPortfolioService service = new PortfolioService(_context);

        PortfolioViewModel? testModel = await service.GetPortfolioFromRouteAsync(username);

        Assert.That(testModel, Is.Null);
    }

    [Test]
    public async Task Test_LogedInUserHasPortfolio_Succeed()
    {
        string userId = "55";
        IPortfolioService service = new PortfolioService(_context);

        bool testResult = await service.LogedInUserHasPortfolio(userId);

        Assert.That(testResult, Is.EqualTo(true));
    }

    [Test]
    [Order(3)]
    public async Task Test_LogedInUserHasPortfolio_Fail()
    {
        string userId = "56"; // user with id 56 has not portfolio
        IPortfolioService service = new PortfolioService(_context);

        bool testResult = await service.LogedInUserHasPortfolio(userId);

        Assert.That(testResult, Is.EqualTo(false));
    }

    [Test]
    public async Task Test_SaveDescriptionAsync_Succeed()
    {
        string userId = "55";
        string userName = "guest";
        IPortfolioService service = new PortfolioService(_context);
        EditDescriptionPortfolioViewModelViewModel? model = 
            await service.GetEditDescriptionViewModelAsync(userId);
        model.Description = "new description";

        await service.SaveDescriptionAsync(model, userId);

        PortfolioViewModel? portfolioModel = await service.GetPortfolioFromRouteAsync(userName);

        Assert.That(portfolioModel, Is.Not.Null);
        Assert.That(portfolioModel.Description, Is.EqualTo("new description"));
    }

    [Test]
    public async Task Test_SaveAboutAsync_Succeed()
    {
        string userId = "55";
        string userName = "guest";
        IPortfolioService service = new PortfolioService(_context);
        EditAboutPortfolioViewModelViewModel? model =
            await service.GetEditAboutViewModelAsync(userId);
        model.About = "new about";

        await service.SaveAboutAsync(model, userId);

        PortfolioViewModel? portfolioModel = await service.GetPortfolioFromRouteAsync(userName);

        Assert.That(portfolioModel, Is.Not.Null);
        Assert.That(portfolioModel.About, Is.EqualTo("new about"));
    }

    [Test]
    public async Task Test_GetAllUsersByRegexAsync_SucceedWithResult()
    {
        string expression = "v";
        IPortfolioService service = new PortfolioService(_context);

        List<PreviewPortfolioViewModel> ports = await service.GetAllUsersByRegexAsync(expression);

        Assert.That(ports, Is.Not.Empty);
    }

    [Test]
    public async Task Test_GetAllUsersByRegexAsync_SucceedWithoutResult()
    {
        string expression = "ww";
        IPortfolioService service = new PortfolioService(_context);

        List<PreviewPortfolioViewModel> ports = await service.GetAllUsersByRegexAsync(expression);

        Assert.That(ports, Is.Empty);
    }
}

using Application.Data;
using Application.Data.Models;
using Application.Services.Interfaces;
using Application.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Application.Web.ViewModels.Portfolio;

namespace UnitTests
{
    [TestFixture]
    public class PortfolioServiceTests
    {
        private ApplicationDbContext _context;
        private readonly List<ApplicationUser> _users;
        private readonly List<Image> _images;

        [OneTimeSetUp]
        public void TestInitialize()
        {
            List<ApplicationUser> users = new List<ApplicationUser>()
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
                        ImageId = null,
                        Image = null,
                        ApplicationUserId = "55"
                    }
                },
                new ApplicationUser()
                {
                    Id = "56",
                    UserName = "Test"
                }
            };

            List<Image> images = new List<Image>()
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
                        82, 0, 0, 2, 0, 0, 0, 2, 0, 8, 2, 0, 0, 0, 123, 26, 67, 173, 0, 0, 0 }
                }
            };

            Project project = new()
            {
                Id = "771",
                Name = "Test project",
                ImageId = null,
                Image = null,
                Description = "Project description",
                Url = null,
                ApplicationUserId = "55",
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
                .UseInMemoryDatabase(databaseName: "InMemoryApplicationDatabase")
                .Options;

            this._context = new ApplicationDbContext(options);
            this._context.Users.AddRange(users);
            this._context.Projects.Add(project);
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
    }
}
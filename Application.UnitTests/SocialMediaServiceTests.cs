using Application.Data.Models;
using Application.Web.ViewModels.SocialMedia;

namespace Application.UnitTests
{
    public class SocialMediaServiceTests
    {
        private ISocialMediaService _service;

        private SocialMedia _socialMedia;
        private ApplicationUser _user;

        private ApplicationDbContext _context;

        [OneTimeSetUp]
        public void TestInitialize()
        {
            this._user = new ApplicationUser()
            {
                Id = "1",
                UserName = "guest"
            };

            this._socialMedia = new SocialMedia()
            {
                Id ="1",
                ApplicationUserId = this._user.Id,
                FacebookUrl = "testUrl",
                ApplicationUser = this._user
            };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemorySocialMediaDb")
                .Options;

            this._context = new ApplicationDbContext(options);

            this._context.Users.Add(this._user);
            this._context.SocialMedias.Add(this._socialMedia);
            this._context.SaveChanges();

            this._service = new SocialMediaService(this._context);
        }

        [Test]
        [Order(1)]
        public async Task GetEditModelByIdAsync_ReturnsViewModel()
        {
            EditSocialMediasViewModel testModel = await this._service.GetEditModelByIdAsync("1");

            Assert.That(testModel, Is.Not.EqualTo(null));
            Assert.That(testModel.Id, Is.EqualTo("1"));
            Assert.That(testModel.FacebookUrl, Is.EqualTo("testUrl"));
        }

        [Test]
        [Order(2)]
        public async Task SaveChangesToModelAsync_Succeed()
        {
            EditSocialMediasViewModel testModel = new()
            {
                Id = "1",
                ApplicationUserId = "1",
                FacebookUrl = "changedUrl"
            };

            await _service.SaveChangesToModelAsync(testModel);

            Assert.That(this._socialMedia.Id, Is.EqualTo("1"));
            Assert.That(this._socialMedia.ApplicationUserId, Is.EqualTo("1"));
            Assert.That(this._socialMedia.ApplicationUser, Is.EqualTo(this._user));
            Assert.That(this._socialMedia.FacebookUrl, Is.EqualTo("changedUrl"));
        }
    }
}

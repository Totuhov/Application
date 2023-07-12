
using Application.Data;
using Application.Data.Models;
using Application.Services;
using Application.Services.Interfaces;
using Application.Web.ViewModels.Image;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class ImageServiceTests
    {
        private IEnumerable<Image> _images;
        private IEnumerable<ApplicationUser> _users;
        private IEnumerable<Portfolio> _portfolios;

        private ApplicationDbContext _context;

        [OneTimeSetUp]
        public void TestInitialize()
        {
            this._users = new List<ApplicationUser>()
            {
                new ApplicationUser()
                {
                    Id = "1",
                    UserName = "guest1"
                }
            };

            this._images = new List<Image>()
            {
                new Image()
                {
                    ImageId = "1",
                    ApplicationUserId = "1",
                    FileExtension = ".png",
                    Bytes = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82, 0, 0, 2, 0, 0, 0, 2, 0, 8, 2, 0, 0, 0, 123, 26, 67, 173, 0, 0, 0}
                }
            };

            this._portfolios = new List<Portfolio>()
            {
                new Portfolio()
                {
                    Id = "1",
                    GreetingsMessage = "Hi",
                    UserDisplayName = "guest1",
                    Description = "Some Random Description",
                    About = "Text about user",
                    ImageId= "2",
                    Image = new Image()
                    {
                        ImageId = "2",
                        ApplicationUserId = "1",
                        FileExtension = ".png",
                        Bytes = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82, 0, 0, 2, 0, 0, 0, 2, 0, 8, 2, 0, 0, 0, 123, 26, 67, 173, 0, 0, 0}
                    },
                    ApplicationUserId= "1",
                }
            };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryApplicationDatabase")
                .Options;

            this._context = new ApplicationDbContext(options);
            this._context.Users.AddRange(this._users);
            this._context.Images.AddRange(this._images);
            this._context.Portfolios.AddRange(this._portfolios);
            this._context.SaveChanges();
        }

        [Test]
        [Order(1)]
        public async Task Test_GetUserImagesAsync()
        {
            IImageService service = new ImageService(this._context);

            List<ImageViewModel> testModels = await service.GetUserImagesAsync("1");

            Assert.That(testModels.Count, Is.EqualTo(2));
        }

        [Test]
        [Order(2)]
        public void Test_GetContentType()
        {
            IImageService service = new ImageService(this._context);

            string contentType = service.GetContentType(".jpg");

            Assert.That(contentType, Is.EqualTo("image/jpeg"));
        }


        [Test]
        [Order(3)]
        public async Task Test_GetImageByIdAsync_Succeed()
        {
            IImageService service = new ImageService(this._context);
            string imageId = "1";

            ImageViewModel? testModel = await service.GetImageByIdAsync(imageId);

            Assert.Multiple(() =>
            {
                Assert.That(testModel.ContentType, Is.EqualTo("image/png"));
                Assert.That(testModel.ImageId, Is.EqualTo("1"));
                Assert.That(testModel.ApplicationUserId, Is.EqualTo("1"));
            });
        }

        [Test]
        [Order(4)]
        public async Task Test_GetImageByIdAsync_Fail()
        {
            IImageService service = new ImageService(this._context);
            string imageId = "6";

            ImageViewModel? testModel = await service.GetImageByIdAsync(imageId);

            Assert.That(testModel, Is.EqualTo(null));

        }

        [Test]
        [Order(7)]
        public async Task Test_DeleteImageByIdAsync()
        {
            IImageService service = new ImageService(this._context);
            string imageId = "1";

            await service.DeleteImageByIdAsync(imageId);
            Image? image = await _context.Images.FirstOrDefaultAsync(i => i.ImageId == imageId);

            Assert.That(image, Is.EqualTo(null));
        }

        [Test]
        [Order(6)]
        public async Task Test_UseImageAsProfilAsync_Succeed()
        {
            IImageService service = new ImageService(this._context);
            string imageId = "1";
            string userId = "1";

            ApplicationUser? user = await this._context.Users.FindAsync("1");

            await service.UseImageAsProfilAsync(imageId, userId);

            Assert.That(user?.Portfolio?.ImageId, Is.EqualTo(imageId));
        }

        [Test]
        [Order(5)]
        public async Task Test_UseImageAsProfilAsync_Fail()
        {
            IImageService service = new ImageService(this._context);
            string imageId = "5";
            string userId = "1";

            ApplicationUser? user = await this._context.Users.FindAsync("1");

            await service.UseImageAsProfilAsync(imageId, userId);

            Assert.That(user?.Portfolio?.ImageId, Is.EqualTo("2"));
        }
    }
}

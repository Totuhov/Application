namespace Application.UnitTests
{
    [TestFixture]
    public class ImageServiceTests
    {
        private IEnumerable<Image> _images;
        private IEnumerable<ApplicationUser> _users;
        private IEnumerable<Portfolio> _portfolios;

        private ApplicationDbContext _context;
        private IImageService _imageService;

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
                    Bytes = new byte[] {137, 80, 78 }
                },
                new Image()
                {
                    ImageId = "4",
                    FileExtension = ".png",
                    Bytes = new byte[] {137, 80, 78 },
                    Characteristic = "defaultProjectImage"
                },
                new Image()
                {
                    ImageId = "3",
                    FileExtension = ".png",
                    Bytes = new byte[] { 137, 80, 78},
                    Characteristic = "defaultProfileImage"
                }
            };

            List<Project> projects = new()
            {
                new()
                {
                    Id = "1",
                    Name = "Test project",
                    ImageId = "1",
                    Description = "Project description",
                    Url = null,
                    ApplicationUserId = "1",
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
                        Bytes = new byte[] { 137, 80, 78}
                    },
                    ApplicationUserId= "1",
                }
            };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryImageServiceDatabase")
                .Options;

            this._context = new ApplicationDbContext(options);
            this._context.Users.AddRange(this._users);
            this._context.Images.AddRange(this._images);
            this._context.Projects.AddRange(projects);
            this._context.Portfolios.AddRange(this._portfolios);
            this._context.SaveChanges();

            this._imageService = new ImageService(this._context);
        }

        [Test]
        [Order(1)]
        public async Task Test_GetUserImagesAsync()
        {
            List<ImageViewModel> testModels = await this._imageService.GetUserImagesAsync("1");

            Assert.That(testModels, Has.Count.EqualTo(2));
        }

        [Test]
        [Order(2)]
        public void Test_GetContentType1()
        {
            string contentType = this._imageService.GetContentType(".jpg");

            Assert.That(contentType, Is.EqualTo("image/jpeg"));
        }

        [Test]
        [Order(2)]
        public void Test_GetContentType2()
        {
            string contentType = this._imageService.GetContentType(".png");

            Assert.That(contentType, Is.EqualTo("image/png"));
        }

        [Test]
        [Order(2)]
        public void Test_GetContentType3()
        {
            string contentType = this._imageService.GetContentType(".gif");

            Assert.That(contentType, Is.EqualTo("image/gif"));
        }

        [Test]
        [Order(2)]
        public void Test_GetContentType_Default()
        {
            string contentType = this._imageService.GetContentType(".");

            Assert.That(contentType, Is.EqualTo("application/octet-stream"));
        }


        [Test]
        [Order(3)]
        public async Task Test_GetImageByIdAsync_Succeed()
        {
            string imageId = "1";

            ImageViewModel testModel = await this._imageService.GetImageByIdAsync(imageId);

            Assert.Multiple(() =>
            {
                Assert.That(testModel.ContentType, Is.EqualTo("image/png"));
                Assert.That(testModel.ImageId, Is.EqualTo("1"));
                Assert.That(testModel.ApplicationUserId, Is.EqualTo("1"));
            });
        }

        [Test]
        [Order(4)]
        public async Task Test_UseImageAsProfilAsync_Succeed()
        {
            string imageId = "1";
            string userId = "1";

            ApplicationUser user = await this._context.Users.FirstAsync(u => u.Id == "1");

            await this._imageService.UseImageAsProfilAsync(imageId, userId);

            Assert.That(user?.Portfolio?.ImageId, Is.EqualTo(imageId));
        }

        [Test]
        [Order(5)]
        public async Task Test_DeleteImageByIdAsync()
        {
            string imageId = "1";

            await this._imageService.DeleteImageByIdAsync(imageId);

            Assert.That(await _context.Images.FirstOrDefaultAsync(i => i.ImageId == imageId), Is.EqualTo(null));
        }
    }
}


using Application.Web.ViewModels.Project;
using Application.Web.ViewModels.SocialMedia;

namespace Application.IntegrationTests
{
    public class SocialMediaControllerTests
    {
        private Mock<ISocialMediaService> mockSocialMediaService;
        private MockControllerContext mockControllerContext;
        private SocialMediaController controller;
        private ClaimsPrincipal user;
        private string username;

        [SetUp]
        public void Initialize()
        {
            this.mockSocialMediaService = new Mock<ISocialMediaService>();
            this.controller = new SocialMediaController(mockSocialMediaService.Object);

            this.user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "guest"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            mockControllerContext = new MockControllerContext(this.user);

            this.controller.ControllerContext = mockControllerContext;
            controller.TempData =
                new TempDataDictionary(mockControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            this.username = "guest";
        }

        [Test]
        public async Task Edit_Get_ValidId_ReturnsView()
        {
            var result = await controller.Edit(this.username) as ActionResult;

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task Edit_Get_Invalid_ReturnsNotFound()
        {            
            var result = await controller.Edit("1") as NotFoundResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task Edit_Post_InvalidModel()
        {
            EditSocialMediasViewModel model = new();
            controller.ModelState.AddModelError("Title", "1");

            var result = await controller.Edit(model);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task Edit_Post_ValidModel()
        {
            EditSocialMediasViewModel model = new()
            {
                Id = "1",
                ApplicationUserId = "1"
            };

            var result = await controller.Edit(model) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Details"));
        }

        [Test]
        public async Task Edit_Post_ReturnsGeneralError()
        {
            EditSocialMediasViewModel model = new();
            this.mockSocialMediaService.Setup(service => service.SaveChangesToModelAsync(model)).Throws<Exception>();


            var result = await controller.Edit(model) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo("Index"));
                Assert.That(result.ControllerName, Is.EqualTo("Home"));
            });
        }
    }
}

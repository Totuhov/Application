
using Application.Web.ViewModels.Image;
using Microsoft.AspNetCore.Mvc;

namespace Application.IntegrationTests
{
    public class ImageControllerTests
    {
        private Mock<IImageService> mockImageService;
        private MockControllerContext mockControllerContext;
        private ImageController controller;
        private ClaimsPrincipal user;
        private CreateImageViewModel model;

        [SetUp]
        public void Initialize()
        {
            mockImageService = new Mock<IImageService>();
            controller = new ImageController(mockImageService.Object);

            this.user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "guest"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            mockControllerContext = new MockControllerContext(this.user);

            this.controller.ControllerContext = mockControllerContext;
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            this.model = new CreateImageViewModel
            {
                File = new FormFile(null, 0, 100, "file", "image.jpg")
            };
        }

        [Test]
        public async Task All_ReturnsNotFound()
        {
            var result = await this.controller.All("notFound") as NotFoundResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task All_RedirectsToView()
        {
            mockImageService.Setup(setup => setup.GetUserImagesAsync("guest")).ReturnsAsync(new List<ImageViewModel>());

            var result = await controller.All("guest") as ActionResult;

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task All_ReturnsGeneralError()
        {
            mockImageService.Setup(x => x.GetUserImagesAsync("1")).Throws<Exception>();

            var result = await controller.All("guest") as RedirectToActionResult;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
                Assert.That(result?.ActionName, Is.EqualTo("Index"));
            });
        }

        [Test]
        public void Create_Get_ReturnsView()
        {
            var result = controller.Create() as ActionResult;

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task Create_Post_ValidModel_RedirectToAction_All()
        {
            var result = await controller.Create(this.model) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo("All"));
                Assert.That(result.RouteValues["id"], Is.EqualTo("guest"));
            });
        }

        [Test]
        public async Task Create_Post_InValidModel_RedirectToAction_All()
        {
             this.model.File = null;
            var result = await controller.Create(this.model) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo("All"));
                Assert.That(result.RouteValues["id"], Is.EqualTo("guest"));
            });
        }

        [Test]
        public async Task Create_Post_ReturnsGeneralError()
        {
            mockImageService.Setup(x => x.SaveImageInDatabaseAsync(this.model, "1")).Throws<Exception>();
            var result = await controller.Create(this.model) as RedirectToActionResult;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
                Assert.That(result?.ActionName, Is.EqualTo("Index"));
            });
        }

        [Test]
        public async Task Delete_NullId_ReturnsNotFound()
        {
            var result = await controller.Delete(null) as NotFoundResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task Delete_InvalidId_ReturnsNotFound()

        {
            var id = "invalidId";

            mockImageService.Setup(x => x.GetImageByIdAsync(id))
                .ReturnsAsync(new ImageViewModel());

            var result = await controller.Delete(id) as NotFoundResult;

            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task Delete_InvalidId_ReturnsGeneralError()
        {
            var id = "invalidId";
            mockImageService.Setup(x => x.GetUserImagesAsync("1")).Throws<Exception>();

            var result = await controller.Delete(id) as RedirectToActionResult;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
                Assert.That(result?.ActionName, Is.EqualTo("Index"));
            });
        }

        [Test]
        public async Task Delete_ValidId_ReturnsViewWithModel()
        {
            var id = "valid_id";

            var expectedModel = new ImageViewModel
            {
                ImageId = id,
            };

            mockImageService.Setup(x => x.GetImageByIdAsync(id))
                .ReturnsAsync(expectedModel);

            var result = await controller.Delete(id) as ViewResult;

            Assert.That(expectedModel, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<ViewResult>());
                Assert.That(result.Model, Is.EqualTo(expectedModel));
            });
        }

        [Test]
        public async Task DeleteConfirmed_ValidId_ReturnsRedirectToAction()
        {
            var id = "valid_id";            
            var imageServiceMock = new Mock<IImageService>();
            imageServiceMock.Setup(x => x.DeleteImageByIdAsync(id));
            
            var result = await controller.DeleteConfirmed(id);

            var successMessage = controller.TempData["SuccessMessage"] as string;
            Assert.That(successMessage, Is.EqualTo("Image deleted successfully!"));
        }

        [Test]
        public async Task DeleteConfirmed_ExceptionThrown_ReturnsGeneralError()
        {
            mockImageService.Setup(x => x.DeleteImageByIdAsync("1")).Throws<Exception>();
            var result = await controller.DeleteConfirmed("1") as RedirectToActionResult;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
                Assert.That(result?.ActionName, Is.EqualTo("Index"));
            });
        }

        [Test]
        public async Task UseAsProfile_ValidId_ReturnsRedirectToAction()
        {
            var id = "valid_id";
            var imageServiceMock = new Mock<IImageService>();
            
            imageServiceMock.Setup(x => x.UseImageAsProfilAsync(id, It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            
            var result = await controller.UseAsProfile(id) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo("Details"));
                Assert.That(result.ControllerName, Is.EqualTo("Portfolio"));
            });
            var successMessage = controller.TempData["SuccessMessage"] as string;
            Assert.That(successMessage, Is.EqualTo("Profile image changed successfully!"));
        }

        [Test]
        public async Task UseAsProfile_ExceptionThrown_ReturnsGeneralError()
        {
            mockImageService.Setup(x => x.UseImageAsProfilAsync("1", "1")).Throws<Exception>();
            
            var result = await controller.UseAsProfile("1") as RedirectToActionResult;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
                Assert.That(result.ActionName, Is.EqualTo("Index"));
            });
        }

    }
}

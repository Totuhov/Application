
using Application.Web.ViewModels.Project;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Controller;

namespace Application.IntegrationTests
{
    public class ProjectControllerTests
    {
        private Mock<IProjectService> mockProjectService;
        private Mock<IUserService> mockUserService;
        private Mock<IImageService> mockImageService;
        private MockControllerContext mockControllerContext;
        private ProjectController controller;
        private ClaimsPrincipal user;
        private CreateImageViewModel imageModel;

        [SetUp]
        public void Initialize()
        {
            this.mockProjectService = new Mock<IProjectService>();
            this.mockUserService = new Mock<IUserService>();
            this.mockImageService = new Mock<IImageService>();
            this.controller = new ProjectController(mockProjectService.Object, mockImageService.Object, mockUserService.Object);

            this.user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "guest"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            this.imageModel = new CreateImageViewModel
            {
                File = new FormFile(null, 0, 100, "file", "image.jpg")
            };

            mockControllerContext = new MockControllerContext(this.user);

            this.controller.ControllerContext = mockControllerContext;
            controller.TempData = new TempDataDictionary(mockControllerContext.HttpContext, Mock.Of<ITempDataProvider>());
        }

        [Test]
        public async Task All_WithWalidId()
        {
            this.mockUserService.Setup(service => service.IsUserExists("1")).ReturnsAsync(true);
            var result = await controller.All("1") as ViewResult;


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.Not.Null);
        }

        [Test]
        public async Task All_WithInvalidId()
        {
            this.mockUserService.Setup(service => service.IsUserExists("1")).ReturnsAsync(false);
            var result = await controller.All("1");

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task All_TrowsExeption()
        {
            this.mockUserService.Setup(service => service.IsUserExists("1")).ReturnsAsync(true);
            this.mockProjectService.Setup(service => service.GetAllProjectsFromUserByUsernameAsync("1")).Throws<Exception>();

            var result = await controller.All("1") as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo("Index"));
                Assert.That(result.ControllerName, Is.EqualTo("Home"));
            });
        }

        [Test]
        public void Create_WithWalidId()
        {
            var result = controller.Create("guest") as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.Not.Null);
        }

        [Test]
        public void Create_WithInvalidId()
        {
            var result = controller.Create("1");

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Create_Post_ValidModel()
        {
            CreateProjectViewModel model = new();

            var result = await controller.Create(model) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("All"));
        }

        [Test]
        public async Task Create_Post_InvalidModel()
        {
            CreateProjectViewModel model = new();

            controller.ModelState.AddModelError("Title", "1");
            var result = await controller.Create(model);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task Create_Post_TrowsExeption()
        {
            CreateProjectViewModel model = new();
            this.mockProjectService.Setup(service => service.SaveProjectForUserAsync("1", model)).Throws<Exception>();

            var result = await controller.Create(model) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo("Index"));
                Assert.That(result.ControllerName, Is.EqualTo("Home"));
            });
        }

        [Test]
        public async Task ChangeImage_ValidId()
        {
            var result = await controller.ChangeImage("guest") as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.Not.Null);
        }

        [Test]
        public async Task ChangeImage_TrowsExeption()
        {
            this.mockImageService.Setup(service => service.GetUserImagesAsync("1")).Throws<Exception>();

            var result = await controller.ChangeImage("2") as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo("Index"));
                Assert.That(result.ControllerName, Is.EqualTo("Home"));
            });
        }

        [Test]
        public async Task ChangeImage_Post_ValidModel()
        {
            ChangeProjectImageViewModel model = new()
            {
                ProjectId = "1",
                ImageId = "2"
            };
            var result = await controller.ChangeImage(model) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("All"));
        }

        [Test]
        public async Task ChangeImage_Post_InvalidModel()
        {
            ChangeProjectImageViewModel model = new();
            var result = await controller.ChangeImage(model) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("All"));
        }

        [Test]
        public async Task ChangeImage_Post_TrowsExeption()
        {
            ChangeProjectImageViewModel model = new()
            {
                ProjectId = "1",
                ImageId = "2"
            };
            this.mockProjectService.Setup(service => service
            .AddImageToProjectAsync(model.ProjectId, model.ImageId)).Throws<Exception>();

            var result = await controller.ChangeImage(model) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo("Index"));
                Assert.That(result.ControllerName, Is.EqualTo("Home"));
            });
        }

        [Test]
        public void CreateImage_ReturnsView()
        {
            var result = controller.CreateImage() as ActionResult;

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task CreateImage_Post_ValidModel()
        {
            var result = await controller.CreateImage(this.imageModel) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo("ChangeImage"));
                Assert.That(result?.RouteValues?["id"], Is.EqualTo("guest"));
            });
        }

        [Test]
        public async Task CreateImage_Post_InvalidModel()
        {
            this.imageModel.File = null;
            var result = await controller.CreateImage(this.imageModel) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo("ChangeImage"));
                Assert.That(result?.RouteValues?["id"], Is.EqualTo("guest"));
            });
        }

        [Test]
        public async Task CreateImage_Post_ReturnsGeneralError()
        {
            mockImageService.Setup(x => x.SaveImageInDatabaseAsync(this.imageModel, "1")).Throws<Exception>();

            var result = await controller.CreateImage(this.imageModel) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo("Index"));
                Assert.That(result.ControllerName, Is.EqualTo("Home"));
            });
        }

        [Test]
        public async Task Edit_ValidId()
        {
            EditProjectViewModel model = new();
            mockProjectService.Setup(setup => setup.GetEditProjectViewModelAsync("1")).ReturnsAsync(model);

            var result = await controller.Edit("1") as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.Not.Null);
        }

        [Test]
        public async Task Edit_InvalidId_ReturnsNotFound()
        {
            EditProjectViewModel model = new();
            mockProjectService.Setup(setup => setup.GetEditProjectViewModelAsync("1")).Throws<Exception>();

            var result = await controller.Edit("1") as NotFoundResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task Edit_Post_ValidModel()
        {
            EditProjectViewModel model = new();

            var result = await controller.Edit(model) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("All"));
        }

        [Test]
        public async Task Edit_Post_InvalidModel()
        {
            EditProjectViewModel model = new();
            controller.ModelState.AddModelError("Title", "1");

            var result = await controller.Edit(model);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task Edit_Post_ReturnsGeneralError()
        {
            EditProjectViewModel model = new();
            this.mockProjectService.Setup(service => service.SaveProjectChangesAsync(model)).Throws<Exception>();


            var result = await controller.Edit(model) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo("Index"));
                Assert.That(result.ControllerName, Is.EqualTo("Home"));
            });
        }

        [Test]
        public async Task Use_ValidId()
        {
            var result = await controller.Use("1") as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("All"));
        }

        [Test]
        public async Task Use_ReturnsGeneralError()
        {
            this.mockImageService.Setup(service => service.UseImageAsProfilAsync("1", "1")).Throws<Exception>();

            var result = await controller.Use("1") as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo("Index"));
                Assert.That(result.ControllerName, Is.EqualTo("Home"));
            });
        }

        [Test]
        public async Task Delete_ValidId()
        {
            ProjectViewModel model = new();
            mockProjectService.Setup(setup => setup.GetCurrentProjectAsync("1")).ReturnsAsync(model);

            var result = await controller.Delete("1") as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.Not.Null);
        }

        [Test]
        public async Task Delete_ReturnsGeneralError()
        {
            mockProjectService.Setup(setup => setup.GetCurrentProjectAsync("1")).Throws<Exception>();

            var result = await controller.Delete("1") as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo("Index"));
                Assert.That(result.ControllerName, Is.EqualTo("Home"));
            });
        }

        [Test]
        public async Task Delete_Post_ValidId()
        {
            ProjectViewModel model = new() 
            { 
                Id = "1",
            };
            mockProjectService.Setup(setup => setup.GetCurrentProjectAsync("1")).ReturnsAsync(model);

            var result = await controller.Delete(model) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("All"));
        }

        [Test]
        public async Task Delete_Post_ReturnsGeneralError()
        {
            ProjectViewModel model = new()
            {
                Id = "1",
            };
            mockProjectService.Setup(setup => setup.DeleteProjectByIdAsync("1")).Throws<Exception>();

            var result = await controller.Delete(model) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo("Index"));
                Assert.That(result.ControllerName, Is.EqualTo("Home"));
            });
        }
    }
}

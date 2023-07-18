
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

        [SetUp]
        public void Initialize()
        {
            this.mockProjectService = new Mock<IProjectService>();
            this.mockUserService = new Mock<IUserService>();
            this.mockImageService = new Mock<IImageService>();
            this.controller = new ProjectController(mockProjectService.Object,mockImageService.Object, mockUserService.Object);

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
    }
}

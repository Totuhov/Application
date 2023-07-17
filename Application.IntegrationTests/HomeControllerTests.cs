
using Application.Data.Models;
using Application.Web.ViewModels;
using Application.Web.ViewModels.Image;
using Application.Web.ViewModels.Portfolio;
using Moq;
using System.Diagnostics;

namespace Application.IntegrationTests
{
    public class HomeControllerTests
    {
        private Mock<IPortfolioService> mockPortfolioService;
        private MockControllerContext mockControllerContext;
        private HomeController controller;
        private ClaimsPrincipal user;
        private Image image;
        private List<PreviewPortfolioViewModel> portfolios;

        [SetUp]
        public void Initialize()
        {
            mockPortfolioService = new Mock<IPortfolioService>();
            controller = new HomeController(mockPortfolioService.Object);

            this.user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "guest"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            this.image = new Image()
            {
                ImageId = "1",
                ApplicationUserId = "1",
                FileExtension = ".png",
                Bytes = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82,
                        0, 0, 2, 0, 0, 0, 2, 0, 8, 2, 0, 0, 0, 123, 26, 67, 173, 0, 0, 0}
            };

            portfolios = new List<PreviewPortfolioViewModel>()
            {
                new PreviewPortfolioViewModel
                {
                    UserName = "guest",
                    ProfileImage = new ImageViewModel()
                    {
                        ImageId = this.image.ImageId,
                        ApplicationUserId = "1",
                        ImageData = Convert.ToBase64String(this.image.Bytes),
                        ContentType = this.image.FileExtension
                    }
                }
            };

            mockControllerContext = new MockControllerContext(this.user);

            this.controller.ControllerContext = mockControllerContext;
            controller.TempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());
        }

        [Test]
        public void Index_UserAuthenticated_RedirectToPortfolioIndex()
        {
            var result = controller.Index() as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo("Index"));
                Assert.That(result.ControllerName, Is.EqualTo("Portfolio"));
            });
        }
        [Test]
        public void Index_UserNotAuthenticated_ReturnsViewResult()
        {
            var userMock = new Mock<ClaimsPrincipal>();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = userMock.Object
                }
            };

            var result = controller.Index();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ActionResult>());
        }

        [Test]
        public void Privacy_ReturnsViewResult()
        {
            // Act
            var result = controller.Privacy() as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task All_EmptyExpression_RedirectToIndex()
        {

            var result = await controller.All(null) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(result.ControllerName, Is.Null);
        }

        [Test]
        public async Task All_NonEmptyExpression_ReturnsViewResultWithModel()
        {
            var expression = "p";
            var expectedModel = this.portfolios;

            mockPortfolioService.Setup(s => s.GetAllUsersByRegexAsync(expression)).ReturnsAsync(expectedModel);

            var result = await controller.All(expression) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.EqualTo(expectedModel));
        }

        [Test]
        public async Task All_ExceptionThrown_ReturnsGeneralError()
        {
            var expression = "p";
            
            mockPortfolioService.Setup(s => s.GetAllUsersByRegexAsync(expression)).Throws<Exception>();
            
            var result = await controller.All(expression) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [Test]
        public void Error_ReturnsViewResultWithModelError()
        {                      
            var result = controller.Error() as ViewResult;

            Assert.That(result, Is.Not.Null);
        }
    }
}

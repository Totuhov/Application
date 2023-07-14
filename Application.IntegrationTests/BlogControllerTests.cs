
using Application.Services.Interfaces;
using Application.Web.Controllers;
using Application.Web.ViewModels.Article;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;

namespace Application.IntegrationTests
{
    [TestFixture]
    public class BlogControllerTests
    {
        private Mock<IBlogService> _mockBlogService;
        private BlogController _controller;

        [OneTimeSetUp]
        public void Initialize()
        {
            _mockBlogService = new Mock<IBlogService>();
            _controller = new BlogController(_mockBlogService.Object);
        }

        [Test]
        public void Create_ValidId_ReturnsView()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "YourUsername") // Set the username or any other claims as needed
            }, "mock"));
            var mockControllerContext = new MockControllerContext(user);
            var id = "YourUsername";

            var testableController = new TestableBlogController(_mockBlogService.Object, "YourUsername"); // Set the desired user ID for testing
            testableController.ControllerContext = mockControllerContext;

            // Manually initialize TempData
            testableController.TempData = new TempDataDictionary(mockControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = testableController.Create(id) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo("Create")); // Verify that the view returned is the "Create" view
                                                        // Additional assertions if needed
        }

        [Test]
        [Order(1)]
        public async Task Create_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "guest")
            }, "mock"));
            var mockControllerContext = new MockControllerContext(user);
            var model = new CreateArticleViewModel
            {
                ApplicationUserId = "guest"
            };
            _controller.TempData = new TempDataDictionary(mockControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = await _controller.Create(model) as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo("Index"));
                Assert.That(result.ControllerName, Is.EqualTo("Portfolio"));
                Assert.That(_controller.TempData["SuccessMessage"], Is.EqualTo("Article was posted successfully!"));
            });
        }

        [Test]
        public async Task Create_InvalidValidModel_RedirectsToView()
        {
            // Arrange            
            var model = new CreateArticleViewModel
            {
                ApplicationUserId = "guest",
            };
            _controller.ModelState.AddModelError("Title", "1");

            // Act
            var result = await _controller.Create(model) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ViewName == null || result.ViewName == "Create", Is.True);
                Assert.That(result.ViewData.ModelState.IsValid, Is.False);
            });
        }

        [Test]
        public async Task Create_ExceptionThrown_ReturnsGeneralError()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "guest")
            }, "mock"));
            var mockControllerContext = new MockControllerContext(user);
            var model = new CreateArticleViewModel();

            _controller.ControllerContext = mockControllerContext;
            _controller.TempData = new TempDataDictionary(mockControllerContext.HttpContext, Mock.Of<ITempDataProvider>());
            _mockBlogService.Setup(service => service.CreatePostAsync(It.IsAny<CreateArticleViewModel>())).ThrowsAsync(new Exception("Some error message"));

            // Act
            var result = await _controller.Create(model) as IActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
        }
    }
    public class MockControllerContext : ControllerContext
    {
        public MockControllerContext(ClaimsPrincipal user)
        {
            HttpContext = new DefaultHttpContext
            {
                User = user,
            };
        }
    }
    public class TestableBlogController : BlogController
    {
        private readonly string _userId;

        public TestableBlogController(IBlogService service, string userId)
            : base(service)
        {
            _userId = userId;
        }

        // Override the GetCurrentUserId() method to return the custom user ID
        protected override string GetCurrentUserId()
        {
            return _userId;
        }
    }
}

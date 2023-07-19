
using Microsoft.AspNetCore.Mvc;

namespace Application.IntegrationTests
{
    public class BaseControllerTests
    {
        private MockControllerContext mockControllerContext;
        private BaseController controller;
        private ClaimsPrincipal user;

        [SetUp]
        public void Initialize()
        {
            this.controller = new BaseController();

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
        public void GetCurrentUserName_Should_Return_UserName()
        {
            var result = this.controller.GetCurrentUserName();

            Assert.That(result, Is.EqualTo("guest"));
        }

        [Test]
        public void GetCurrentUserId_Should_Return_UserId()
        {
            var result = controller.GetCurrentUserId();

            Assert.That(result, Is.EqualTo("1"));
        }

        [Test]
        public void GeneralError_Should_Return_RedirectToAction_Result()
        {
            var result = controller.GeneralError();

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirectResult = (RedirectToActionResult)result;
            Assert.Multiple(() =>
            {
                Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
                Assert.That(redirectResult.ControllerName, Is.EqualTo("Home"));
            });
        }
    }
}

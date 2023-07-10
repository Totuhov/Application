

using Application.Services.Interfaces;
using Application.Web.Controllers;
using Moq;

namespace UnitTests
{
    public class PortfolioControllerTests
    {
        [Fact]
        public async Task Index_ReturnsView_WhenUserDoesNotHavePortfolio()
        {
            // Arrange
            var userId = "1"; // Replace with appropriate user ID
            var userName = "testuser"; // Replace with appropriate user name

            var portfolioServiceMock = new Mock<IPortfolioService>();
            portfolioServiceMock.Setup(s => s.LogedInUserHasPortfolio(It.IsAny<string>())).ReturnsAsync(false);

            var controller = new PortfolioController(portfolioServiceMock.Object);
            controller.SetCurrentUserId(userId);
            controller.SetCurrentUserName(userName);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CreatePortfolioViewModel>(viewResult.Model);
            Assert.Equal(userId, model.Id);
            Assert.Equal(userName, model.UserName);
        }

        [Fact]
        public async Task Index_ReturnsRedirectToAction_WhenUserHasPortfolio()
        {
            // Arrange
            var userName = "testuser"; // Replace with appropriate user name

            var portfolioServiceMock = new Mock<IPortfolioService>();
            portfolioServiceMock.Setup(s => s.LogedInUserHasPortfolio(It.IsAny<string>())).ReturnsAsync(true);

            var controller = new PortfolioController(portfolioServiceMock.Object);
            controller.SetCurrentUserName(userName);

            // Act
            var result = await controller.Index();

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectToActionResult.ActionName);
            Assert.Equal(userName, redirectToActionResult.RouteValues["id"]);
        }

        [Fact]
        public async Task Index_ReturnsGeneralError_WhenExceptionOccurs()
        {
            // Arrange
            var portfolioServiceMock = new Mock<IPortfolioService>();
            portfolioServiceMock.Setup(s => s.LogedInUserHasPortfolio(It.IsAny<string>())).ThrowsAsync(new Exception());

            var controller = new PortfolioController(portfolioServiceMock.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var generalErrorResult = Assert.IsType<GeneralErrorResult>(result);
            // Add additional assertions specific to the GeneralErrorResult if needed
        }
    }
}

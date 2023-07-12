
using System.Net;

namespace Application.IntegrationTests
{
    public class HomeControllerTests
    {
        [Test]
        public async Task Index()
        {
            // Arrange
            var httpClient = new HttpClient();
            // Act
            var response = await httpClient
            .GetAsync("https://localhost:7288/");
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Privacy()
        {
            // Arrange
            var httpClient = new HttpClient();
            // Act
            var response = await httpClient
            .GetAsync("https://localhost:7288/Home/Privacy");
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task All()
        {
            // Arrange
            var httpClient = new HttpClient();
            // Act
            var response = await httpClient
            .GetAsync("https://localhost:7288/Home/All?expression=p");
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }
}
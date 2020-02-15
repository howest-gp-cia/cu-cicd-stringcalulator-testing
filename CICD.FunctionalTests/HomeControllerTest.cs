using System.Threading.Tasks;
using System.Net.Http;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using CICD.Mvc;

namespace CICD.FunctionalTests
{
    public class HomeControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        protected HttpClient Client { get; }

        public HomeControllerTests(WebApplicationFactory<Startup> factory)
        {
            Client = factory.CreateClient();
        }

        [Fact]
        public async Task Index_Get_ReturnsSampleCalculationAsync()
        {
            // Arrange
            var response = await Client.GetAsync("/");
            response.EnsureSuccessStatusCode();

            // Act
            var stringResponse = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Contains("Sum: 6", stringResponse);
        }

        [Fact]
        public async Task Calculate_Get_ReturnsCorrectResultAsync()
        {
            // Arrange
            string input = "10,20,30";
            var response = await Client.GetAsync($"/Home/Calculate?input={input}");
            response.EnsureSuccessStatusCode();

            // Act
            var stringResponse = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Contains("Sum: 60", stringResponse);
        }

        [Fact]
        public async Task Calculate_Get_ReturnsEmailButtonAsync()
        {
            // Arrange
            var response = await Client.GetAsync("/");
            response.EnsureSuccessStatusCode();

            // Act
            var stringResponse = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Contains("<input type=\"submit\" value=\"email\"", stringResponse);
        }
    }
}

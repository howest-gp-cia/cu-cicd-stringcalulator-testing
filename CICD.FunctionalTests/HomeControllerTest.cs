using System.Threading.Tasks;
using System.Net.Http;
using Xunit;

namespace CICD.FunctionalTests
{
    public class HomeControllerTest: IClassFixture<WebTestFixture>
    {
        public HomeControllerTest(WebTestFixture factory)
        {
            Client = factory.CreateClient();
        }

        public HttpClient Client { get; }

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

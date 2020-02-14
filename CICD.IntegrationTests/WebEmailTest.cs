using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using FluentAssertions.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CICD.IntegrationTests
{
    public class WebEmailTest
    {
        public const string mailHogApiRoot = "http://mail:8025/api";
        public const string mvcAppRoot = "http://mvcapp";

        private readonly ITestOutputHelper output;

        public WebEmailTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async Task HomeController_SendEmailWithCalculation_EmailIsReceived()
        {
            // Arrange
            string mvcEmailController = $"{mvcAppRoot}/Home/Email?input='1,2,3'";
            await DeleteAllMailMessages();
            var client = new HttpClient();
            var sendEmail = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{mvcEmailController}")
            };

            // ACT
            output.WriteLine($"Sending email: {sendEmail.RequestUri}");
            using (var response = await client.SendAsync(sendEmail))
            {
                if (response.IsSuccessStatusCode)
                {
                    output.WriteLine($"Email sent\n");
                }
            }

            // ASSERT
            var checkEmails = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{mailHogApiRoot}/v2/messages")
            };
            output.WriteLine($"Checking email: {checkEmails.RequestUri}");
            using (var response = await client.SendAsync(checkEmails))
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var messages = JObject.Parse(content);
                messages.Should().HaveElement("total").Which.Should().HaveValue("1");
                messages.Should().HaveElement("items")
                    .Which.Should().BeOfType<JArray>()
                    .Which.First.Should().HaveElement("Raw")
                    .Which.Should().HaveElement("From")
                    .Which.Should().HaveValue("cicdsolution@howestgp.be");
                output.WriteLine($"Email checked!");
            }
        }

        private async Task DeleteAllMailMessages()
        {
            var client = new HttpClient();

            var deleteEmails = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{mailHogApiRoot}/v1/messages")

            };

            output.WriteLine($"Deleting emails: {deleteEmails.RequestUri}");
            using (var response = await client.SendAsync(deleteEmails))
            {
                if (response.IsSuccessStatusCode)
                {
                    output.WriteLine("Emails deleted\n");
                }
            }
        }
    }
}

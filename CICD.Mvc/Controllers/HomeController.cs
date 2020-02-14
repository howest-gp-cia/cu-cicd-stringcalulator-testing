using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CICD.Mvc.Models;
using CICD.Mvc.ViewModels;
using CICD.Domain;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;

namespace CICD.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        public IActionResult Index()
        {

            return Calculate("1,2,3");
        }


        public  IActionResult Calculate(string input)
        {
            StringCalculatorViewModel calculationResult = CalculateSum(input);
            return View("Index", calculationResult);
        }


        public async Task<IActionResult> Email(string input)
        {
            StringCalculatorViewModel calculationResult = CalculateSum(input);
            await SendMail(calculationResult);
            return View("Index", calculationResult);
        }

        private async Task SendMail(StringCalculatorViewModel content)
        {
            const string MAIL_HOST = "mail";
            const int MAIL_PORT = 1025;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("CICD Web Application", "cicdsolution@howestgp.be"));
            message.To.Add(new MailboxAddress("Student", "student@sumwanted.com"));
            message.Subject = "Your calculated Sum";
            message.Body = new TextPart("plain")
            {
                Text = $"Hello, your calculation result: sum of {content.Input} = {content.Sum} " +
                $"{(content.Error != String.Empty ? $"\n\nError: {content.Error} " : "")}"
            };
            using (var mailClient = new SmtpClient())
            {
                await mailClient.ConnectAsync(MAIL_HOST, MAIL_PORT, SecureSocketOptions.None);
                await mailClient.SendAsync(message);
                await mailClient.DisconnectAsync(true);
                content.EmailIsSent = true;
            }

            

        }

        private StringCalculatorViewModel CalculateSum(string input)
        {
            StringCalculatorViewModel scvm = new StringCalculatorViewModel
            {
                Input = input
            };
            try
            {
                StringCalculator stringCalculator = new StringCalculator();
                int sum = stringCalculator.Add(input);
                scvm.Sum = sum;
            }
            catch (Exception ex)
            {
                scvm.Sum = 0;
                scvm.Error = ex.Message;
            }
            return scvm;

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

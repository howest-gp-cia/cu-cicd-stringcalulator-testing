using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CICD.Mvc.Models;
using CICD.Mvc.ViewModels;
using CICD.Domain;

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

            return Index("1,2,3");
        }

        [HttpPost("input")]
        public IActionResult Index(string input)
        {
            return View(CalculateSum(input));
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

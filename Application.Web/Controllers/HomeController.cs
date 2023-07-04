using Application.Services.Interfaces;
using Application.Web.ViewModels;
using Application.Web.ViewModels.Portfolio;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Application.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPortfolioService _portfolioService;

        public HomeController(ILogger<HomeController> logger, IPortfolioService portfolioService)
        {
            this._logger = logger;
            this._portfolioService = portfolioService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Portfolio");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> All(string expression)
        {
            if (String.IsNullOrEmpty(expression))
            {
                return RedirectToAction("Index");
            }
            
            List<PreviewPortfolioViewModel> model = await _portfolioService.GetAllUsersByRegexAsync(expression);

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
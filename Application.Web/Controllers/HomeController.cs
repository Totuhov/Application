using Application.Services.Interfaces;
using Application.Web.ViewModels;
using Application.Web.ViewModels.Portfolio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Application.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IPortfolioService _portfolioService;

        public HomeController(IPortfolioService portfolioService)
        {
            this._portfolioService = portfolioService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                if (User?.Identity?.IsAuthenticated == true)
                {
                    return RedirectToAction("Index", "Portfolio");
                }
                return View();
            }
            catch (Exception)
            {
                return GeneralError();
            }

        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> All(string expression)
        {
            try
            {
                if (String.IsNullOrEmpty(expression))
                {
                    return RedirectToAction("Index");
                }

                List<PreviewPortfolioViewModel> model = await _portfolioService.GetAllUsersByRegexAsync(expression);

                return View(model);
            }
            catch (Exception)
            {

                return GeneralError();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
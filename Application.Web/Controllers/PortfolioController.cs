
namespace Application.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Application.Services.Interfaces;
using Application.Web.ViewModels.Portfolio;

using static Application.Common.NotificationMessagesConstants;
using Application.Web.ViewModels.ContactForm;

public class PortfolioController : BaseController
{

    private readonly IPortfolioService _portfolioService;
    private readonly IUserService _userService;
    private readonly IMessageService _messageService;

    public PortfolioController(IPortfolioService portfolioService, IUserService userService, IMessageService messageService)
    {
        _portfolioService = portfolioService;
        _userService = userService;
        _messageService = messageService;
    }


    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            if (!await _portfolioService.LogedInUserHasPortfolio(GetCurrentUserId()))
            {
                CreatePortfolioViewModel model = new()
                {
                    Id = GetCurrentUserId(),
                    UserName = GetCurrentUserName()
                };

                return View(model);
            }

            return RedirectToAction("Details", new { id = GetCurrentUserName() });
        }
        catch (Exception)
        {

            return this.GeneralError();
        }
    }
    [AllowAnonymous]
    public async Task<IActionResult> Details(string id)
    {
        if (await _userService.IsUserExists(id))
        {
            PortfolioViewModel model = await _portfolioService.GetPortfolioFromRouteAsync(id);

            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        return NotFound();
    }

    public async Task<IActionResult> Create(string id)
    {
        if (GetCurrentUserName() == id && !await _portfolioService.LogedInUserHasPortfolio(GetCurrentUserId()))
        {
            await _portfolioService.CreateFirstPortfolioAsync(GetCurrentUserId());
        }

        return RedirectToAction("Details", new { id });
    }

    [HttpGet]
    public async Task<IActionResult> EditDescription(string id)
    {
        if (id != GetCurrentUserName())
        {
            return RedirectToAction("Error", "Home");
        }

        EditDescriptionPortfolioViewModelViewModel model = await _portfolioService.GetEditDescriptionViewModelAsync(GetCurrentUserId());

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditDescription(EditDescriptionPortfolioViewModelViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _portfolioService.SaveDescriptionAsync(model, GetCurrentUserId());

        return RedirectToAction("Details", new { id = GetCurrentUserName() });
    }

    [HttpGet]
    public async Task<IActionResult> EditAbout(string id)
    {
        if (id != GetCurrentUserName())
        {
            return RedirectToAction("Error", "Home");
        }

        EditAboutPortfolioViewModelViewModel model = await _portfolioService.GetEditAboutViewModelAsync(GetCurrentUserId());

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditAbout(EditAboutPortfolioViewModelViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _portfolioService.SaveAboutAsync(model, GetCurrentUserId());

        return RedirectToAction("Details", new { id = GetCurrentUserName() });
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> SendEmail(string id)
    {
        ContactFormViewModel model = new()
        {
            RecieverEmail = await _userService.GetUserEmailByUsernameAsync(id),
            RecieverUserName = id
        };

        return View(model);
    }

    [AllowAnonymous]
    [HttpPost]
    public IActionResult SendEmail(ContactFormViewModel model, string id)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string senderName = Request.Form["senderName"];
            string senderEmail = Request.Form["senderEmail"];
            string text = Request.Form["text"];
            string recieverEmail = Request.Form["recieverEmail"];

            _messageService.SendEmail(recieverEmail, senderName, senderEmail, text);

            this.TempData[SuccessMessage] = "Message was send successfily!";

            return RedirectToAction("Details", new {id});

        }
        catch (Exception)
        {

            this.TempData[ErrorMessage] = "Message was not send! Try again later";
            return RedirectToAction("GeneralError");
        }
    }

    [AllowAnonymous]
    private IActionResult GeneralError()
    {
        this.TempData[ErrorMessage] =
            "Unexpected error occurred! Please try again later or contact administrator";

        return this.RedirectToAction("Index", "Home");
    }

}

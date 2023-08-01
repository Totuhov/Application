
namespace Application.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Application.Services.Interfaces;
using Application.Web.ViewModels.Portfolio;
using Application.Web.ViewModels.ContactForm;

using static Application.Common.NotificationMessagesConstants;

public class PortfolioController : BaseController
{

    private readonly IPortfolioService _portfolioService;
    private readonly IUserService _userService;
    private readonly IMessageService _messageService;

    public PortfolioController(IPortfolioService portfolioService, 
        IUserService userService, 
        IMessageService messageService)
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

            return GeneralError();
        }
    }
    [AllowAnonymous]
    public async Task<IActionResult> Details(string id)
    {
        try
        {
            if (await _userService.IsUserExists(id))
            {
                PortfolioViewModel model = await _portfolioService.GetPortfolioFromRouteAsync(id);

                return View(model);
            }

            return NotFound();
        }
        catch (Exception)
        {

            return GeneralError();
        }
    }

    [Authorize(Roles = "User")]
    public async Task<IActionResult> Create(string id)
    {
        try
        {
            if (GetCurrentUserName() == id && !await _portfolioService.LogedInUserHasPortfolio(GetCurrentUserId()))
            {
                await _portfolioService.CreateFirstPortfolioAsync(GetCurrentUserId());
            }

            return RedirectToAction("Details", new { id });
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditDescription(string id)
    {
        try
        {
            if (GetCurrentUserName() != id)
            {
                return NotFound();
            }

            EditDescriptionPortfolioViewModel? model = await _portfolioService.GetEditDescriptionViewModelAsync(GetCurrentUserId());
            return View(model);
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditDescription(EditDescriptionPortfolioViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _portfolioService.SaveDescriptionAsync(model, GetCurrentUserId());
            this.TempData[SuccessMessage] = "Changes was saved.";
            return RedirectToAction("Details", new { id = GetCurrentUserName() });
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditAbout(string id)
    {
        try
        {
            if (id != GetCurrentUserName())
            {
                return NotFound();
            }

            EditAboutPortfolioViewModelViewModel model = await _portfolioService.GetEditAboutViewModelAsync(GetCurrentUserId());
            return View(model);
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditAbout(EditAboutPortfolioViewModelViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        try
        {
            await _portfolioService.SaveAboutAsync(model, GetCurrentUserId());
            this.TempData[SuccessMessage] = "Changes was saved.";
            return RedirectToAction("Details", new { id = GetCurrentUserName() });
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> SendEmail(string id)
    {
        try
        {
            if (await _userService.IsUserExists(id))
            {
                ContactFormViewModel model = new()
                {
                    RecieverEmail = await _userService.GetUserEmailByUsernameAsync(id),
                    RecieverUserName = id
                };

                return View(model);
            }
            return NotFound();
        }
        catch (Exception)
        {

            return GeneralError();
        }
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

            return RedirectToAction("Details", new { id });
        }
        catch (Exception)
        {
            return RedirectToAction("GeneralError");
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

using static Application.Common.NotificationMessagesConstants;

namespace Application.Web.Controllers;

[Authorize]
public class BaseController : Controller
{
    protected string GetCurrentUserName()
    {
        return User.FindFirstValue(ClaimTypes.Name);
    }

    protected virtual string GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    [AllowAnonymous]
    protected IActionResult GeneralError()
    {
        this.TempData[ErrorMessage] =
            "Unexpected error occurred! Please try again later or contact administrator";

        return this.RedirectToAction("Index", "Home");
    }
}

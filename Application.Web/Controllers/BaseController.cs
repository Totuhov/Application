using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Application.Web.Controllers;

[Authorize]
public class BaseController : Controller
{
    protected string GetCurrentUserName()
    {
        return User.FindFirstValue(ClaimTypes.Name);
    }

    protected string GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}


namespace Application.Web.Controllers;

using Microsoft.AspNetCore.Mvc;

using Application.Services;
using Application.Web.ViewModels.SocialMedia;

using Application.Services.Interfaces;

using static Application.Common.NotificationMessagesConstants;
public class SocialMediaController : BaseController
{
    private readonly ISocialMediaService _socialMediaService;

    public SocialMediaController(ISocialMediaService socialMediaService)
    {
        _socialMediaService = socialMediaService;
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string username)
    {
        EditSocialMediasViewModel model = await _socialMediaService.GetEditModelByIdAsync(GetCurrentUserId());

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditSocialMediasViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _socialMediaService.SaveChangesToModelAsync(model);
            this.TempData[SuccessMessage] = "Links' changes was saved";
            return RedirectToAction("Details", "Portfolio", new {id = GetCurrentUserName()});
        }
        catch (Exception)
        {

            return GeneralError();
        }

    }
}

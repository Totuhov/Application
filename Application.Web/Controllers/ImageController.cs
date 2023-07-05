
namespace Application.Web.Controllers;

using Microsoft.AspNetCore.Mvc;

using Application.Services.Interfaces;
using Application.Web.ViewModels.Image;

using static Application.Common.NotificationMessagesConstants;

public class ImageController : BaseController
{
    private readonly IImageService _imageService;

    public ImageController(IImageService imageService)
    {
        this._imageService = imageService;
    }


    [HttpGet]
    public async Task<IActionResult> All(string id)
    {
        if (id != GetCurrentUserName())
        {
            return RedirectToAction("Error", "Home");
        }

        var model = await _imageService.GetUserImagesAsync(GetCurrentUserId());

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateImageViewModel model)
    {

        if (model.File != null && model.File.Length > 0)
        {
            await _imageService.SaveImageInDatabaseAsync(model, GetCurrentUserId());
            this.TempData[SuccessMessage] = "Image added successfily!";
        }

        return RedirectToAction("All", new { id = GetCurrentUserName() });

    }

    public async Task<IActionResult> Delete(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var model = await _imageService.GetImageByIdAsync(id);

        if (model.ImageId == null)
        {
            return NotFound();
        }

        return View(model);
    }

    // POST: Image/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {

        await _imageService.DeleteImageByIdAsync(id);

        return RedirectToAction("All", new { id = GetCurrentUserName() });
    }

    public async Task<IActionResult> UseAsProfile(string id)
    {

        await _imageService.UseImageAsProfilAsync(id, GetCurrentUserId());

        return RedirectToAction("Details", "Portfolio", new { id = GetCurrentUserName() });
    }
}

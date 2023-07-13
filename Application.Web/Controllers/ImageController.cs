﻿
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
            return NotFound();
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
    [ValidateAntiForgeryToken]
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

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        try
        {
            await _imageService.DeleteImageByIdAsync(id);
            this.TempData[SuccessMessage] = "Image deleted successfily!";
            return RedirectToAction("All", new { id = GetCurrentUserName() });
        }
        catch (Exception)
        {
            this.TempData[ErrorMessage] = "Something's. Image was not deleted. Try again later or contact administrator";
            return RedirectToAction("All", new { id = GetCurrentUserName() });
        }

    }

    public async Task<IActionResult> UseAsProfile(string id)
    {
        try
        {
            await _imageService.UseImageAsProfilAsync(id, GetCurrentUserId());
            this.TempData[SuccessMessage] = "Profile image change successfily!";

            return RedirectToAction("Details", "Portfolio", new { id = GetCurrentUserName() });
        }
        catch (Exception)
        {
            this.TempData[ErrorMessage] = "Something's. Image was not changed successfuly. Try again later or contact administrator";
            return RedirectToAction("Details", "Portfolio", new { id = GetCurrentUserName() });
        }
    }
}

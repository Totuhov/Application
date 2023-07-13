
namespace Application.Web.Controllers;

using Microsoft.AspNetCore.Mvc;

using Application.Services.Interfaces;
using Application.Web.ViewModels.Image;
using Application.Web.ViewModels.Project;

using static Application.Common.NotificationMessagesConstants;
using Microsoft.AspNetCore.Authorization;

public class ProjectController : BaseController
{
    private readonly IProjectService _projectService;
    private readonly IImageService _imageService;
    private readonly IUserService _userService;

    public ProjectController(IProjectService projectService, IImageService imageService, IUserService userService)
    {
        this._projectService = projectService;
        this._imageService = imageService;
        this._userService = userService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> All(string id)
    {
        if (await _userService.IsUserExists(id))
        {
            AllProjectsViewModel model = new()
            {
                UserName = id,
                Projects = await _projectService.GetAllProjectsFromUserByUsernameAsync(id)

            };
            return View(model);
        }

        return NotFound();

    }

    [HttpGet]
    public IActionResult Create(string id)
    {
        CreateProjectViewModel model = new()
        {
            ApplicationUserId = GetCurrentUserId()
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _projectService.SaveProjectForUserAsync(GetCurrentUserId(), model);
            this.TempData[SuccessMessage] = "Project was added successfily!";
            return RedirectToAction("All", new { id = GetCurrentUserName() });
        }
        catch (Exception)
        {

            this.TempData[ErrorMessage] = "Somethig's wrong. Creating a new project was unsuccessful!";
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> ChangeImage(string id)
    {
        ChangeProjectImageViewModel model = new()
        {
            ProjectId = id,
            Images = await _imageService.GetUserImagesAsync(GetCurrentUserId())
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ChangeImage(ChangeProjectImageViewModel model)
    {

        if (model.ProjectId != null && model.ImageId != null)
        {
            await _projectService.AddImageToProjectAsync(model.ProjectId, model.ImageId);
        }

        return RedirectToAction("All", new { id = GetCurrentUserName() });
    }

    public IActionResult CreateImage()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateImage([FromForm] CreateImageViewModel model)
    {

        if (model.File != null && model.File.Length > 0)
        {
            await _imageService.SaveImageInDatabaseAsync(model, GetCurrentUserId());

            // Redirect or show success message to the user
        }
        // Handle validation errors if necessary
        return RedirectToAction("All", new { id = GetCurrentUserName() });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        try
        {
            EditProjectViewModel model = await _projectService.GetEditProjectViewModelAsync(id);

            model.ApplicationUserId = GetCurrentUserId();
            return View(model);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditProjectViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _projectService.SaveProjectChangesAsync(model);
            return RedirectToAction("All", new { id = GetCurrentUserName() });
        }
        catch (Exception)
        {
            this.TempData[ErrorMessage] = "Somethig's wrong. Project was not saved successfuly.";
            return RedirectToAction("Index", "Home");
        }
        
    }

    public async Task<IActionResult> Use(string id)
    {
        try
        {
            await _imageService.UseImageAsProfilAsync(id, GetCurrentUserId());
            return RedirectToAction("Details", "Portfolio", new { id = GetCurrentUserName() });
        }
        catch (Exception)
        {

            this.TempData[ErrorMessage] = "Somethig's wrong. Image was not added to profile successfuly.";
            return RedirectToAction("Index", "Home");
        }        
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            ProjectViewModel model = await _projectService.GetCurrentProjectAsync(id);
            return View(model);
        }
        catch (Exception)
        {
            return NotFound();
        }                 
    }

    [HttpPost]
    public async Task<IActionResult> Delete(ProjectViewModel model)
    {
        try
        {
            await _projectService.DeleteProjectByIdAsync(model.Id);
            this.TempData[SuccessMessage] = "Project was deleted successfily!";
            return RedirectToAction("All", new { id = GetCurrentUserName() });
        }
        catch (Exception)
        {
            this.TempData[ErrorMessage] = "Somethig's wrong. Delete was unsuccessful!";
            return View(model);
        }
    }
}

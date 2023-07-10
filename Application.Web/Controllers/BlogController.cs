
namespace Application.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Application.Services.Interfaces;
using Application.Web.ViewModels.Article;

using static Application.Common.NotificationMessagesConstants;

public class BlogController : BaseController
{
    private readonly IBlogService _service;
    private readonly IUserService _userService;

    public BlogController(IBlogService service, IUserService userService)
    {
        _service = service;
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Create(string id)
    {
        try
        {
            if (id == GetCurrentUserName())
            {
                CreateArticleViewModel model = new()
                {
                    ApplicationUserId = GetCurrentUserId()
                };

                return View(model);
            }

            return NotFound();
        }
        catch (Exception)
        {
            return NotFound();
        }        
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateArticleViewModel model)
    {

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _service.CreatePostAsync(model);
            this.TempData[SuccessMessage] = "Article was posted successfily!";
            return RedirectToAction("Index", "Portfolio");
        }
        catch (Exception)
        {
            this.TempData[ErrorMessage] = "Article was not posted successfily!";
            return RedirectToAction("Index", "Portfolio");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {

        CreateArticleViewModel model = await _service.GetCreateArticleViewModelByIdAsync(id, GetCurrentUserId());

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(CreateArticleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _service.SavePostAsync(model);

        string? userName = await _userService.GetUsernameByIdAsync(model.ApplicationUserId);

        if (userName == null)
        {
            return NotFound();
        }

        return RedirectToAction("All", new { id = userName });
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        ArticleViewModel model = await _service.GetArticleViewModelByIdAsync(id);

        return View(model);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> All(string id)
    {
        try
        {
            List<ArticleViewModel> articles = await _service.GetAllArticlesByUserNameAsync(id);

            AllArticlesViewModel model = new()
            {
                Articles = articles,
                UserName = id
            };
            return View(model);
        }
        catch (Exception)
        {

            return RedirectToAction("Index", "Home");
        }
    }


    [HttpGet]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            ArticleViewModel model = await _service.GetArticleViewModelByIdAsync(id);

            await _service.DeleteArteicleAsync(model);

            this.TempData[SuccessMessage] = "Article was deleted successfily!";
            return RedirectToAction("Details", "Portfolio", new { id = model.ApplicationUserName });
        }
        catch (Exception)
        {

            this.TempData[ErrorMessage] = "Something wrong. Article was not deleted successfily!";
            return RedirectToAction("Details", "Portfolio");
        }
    }
}


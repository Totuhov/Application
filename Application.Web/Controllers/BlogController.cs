
namespace Application.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Application.Services.Interfaces;
using Application.Web.ViewModels.Article;

using static Application.Common.NotificationMessagesConstants;

public class BlogController : BaseController
{
    private readonly IBlogService _blogService;

    public BlogController(IBlogService service)
    {
        _blogService = service;
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

            return GeneralError();
        }
        catch (Exception)
        {
            return GeneralError();
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
            await _blogService.CreatePostAsync(model);
            this.TempData[SuccessMessage] = "Article was posted successfuly!";
            return RedirectToAction("Index", "Portfolio");
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        try
        {
            bool result = _blogService.IsUserOwnerOfArticle(id, GetCurrentUserId());

            if (result || User.IsInRole("Admin"))
            {
                CreateArticleViewModel model = await _blogService.GetCreateArticleViewModelByIdAsync(id, GetCurrentUserId());
                return View(model);
            }
            return GeneralError();
        }
        catch (Exception)
        {
            return GeneralError();
        }

    }

    [HttpPost]
    public async Task<IActionResult> Edit(CreateArticleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _blogService.SavePostAsync(model);
            string userName = await _blogService.GetUsernameByArticleIdAsync(model.Id);

            if (userName == null)
            {
                return GeneralError();
            }
            this.TempData[SuccessMessage] = "Article was edited successfuly!";
            return RedirectToAction("All", new { id = userName });
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        try
        {
            ArticleViewModel model = await _blogService.GetArticleViewModelByIdAsync(id);
            return View(model);
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> All(string id)
    {
        try
        {
            List<ArticleViewModel> articles = await _blogService.GetAllArticlesByUserNameAsync(id);

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
            bool result = _blogService.IsUserOwnerOfArticle(id, GetCurrentUserId());

            if (result || User.IsInRole("Admin"))
            {
                ArticleViewModel model = await _blogService.GetArticleViewModelByIdAsync(id);

                await _blogService.DeleteArticleAsync(model);

                this.TempData[SuccessMessage] = "Article was deleted successfuly!";
                return RedirectToAction("Details", "Portfolio", new { id = model.ApplicationUserName });
            }

            return GeneralError();
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }
}



namespace Application.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Application.Services.Interfaces;
using Application.Web.ViewModels.Article;

[AllowAnonymous]
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

    [HttpPost]
    public async Task<IActionResult> Create(CreateArticleViewModel model)
    {

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _service.CreatePostAsync(model);

        return RedirectToAction("Index", "Portfolio");
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

        return RedirectToAction("All", new {id = userName});
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        ArticleViewModel model = await _service.GetArticleViewModelByIdAsync(id);

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> All(string id)
    {
        
        List<ArticleViewModel> articles = await _service.GetAllArticlesByUserNameAsync(id);

        AllArticlesViewModel model = new()
        {
            Articles = articles,
            UserName = id
        };
        return View(model);
    }


    [HttpGet]
    public async Task<IActionResult> Delete(string id)
    {
        ArticleViewModel model = await _service.GetArticleViewModelByIdAsync(id);

        await _service.DeleteArteicleAsync(model);

        return RedirectToAction("Index", "Portfolio");
    }

    //[HttpPost]
    //public async Task<IActionResult> Delete(ArticleViewModel model)
    //{
    //    await _service.DeleteArteicleAsync(model);

    //    return RedirectToAction("All");
    //}
}


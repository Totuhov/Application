
namespace Application.Services;

using Application.Data;
using Application.Data.Models;
using Application.Services.Interfaces;
using Application.Web.ViewModels.Article;
using Microsoft.EntityFrameworkCore;

public class BlogService : IBlogService
{
    private readonly ApplicationDbContext _context;

    public BlogService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CreateArticleViewModel> GetCreateArticleViewModelByIdAsync(string articleId, string userId)
    {
        Article article = await _context.Articles.FirstAsync(a => a.Id == articleId);

        return new CreateArticleViewModel()
        {
            Id = article.Id,
            Title = article.Title,
            EditedOn = DateTime.Now,
            Content = article.Content,
            ApplicationUserId = userId
        };

    }

    public async Task CreatePostAsync(CreateArticleViewModel model)
    {
        Article article = new()
        {
            Title = model.Title,
            Content = model.Content,
            EditedOn = DateTime.Now,
            ApplicationUserId = model.ApplicationUserId
        };

        await _context.Articles.AddAsync(article);
        await _context.SaveChangesAsync();
    }

    public async Task SavePostAsync(CreateArticleViewModel model)
    {
        Article article = await _context.Articles.FirstAsync(a => a.Id == model.Id);

        article.Title = model.Title;
        article.EditedOn = DateTime.Now;
        article.Content = model.Content;

        await _context.SaveChangesAsync();
    }

    public async Task<ArticleViewModel> GetArticleViewModelByIdAsync(string id)
    {
        Article article = await _context.Articles.FirstAsync(a => a.Id == id);

        return new ArticleViewModel()
        {
            Id = article.Id,
            Title = article.Title,
            CreatedOn = article.CreatedOn,
            EditedOn = article.EditedOn,
            Content = article.Content,
            ApplicationUserName = article.ApplicationUser.UserName,
            IsDeleted = article.IsDeleted
        };
    }

    public async Task<List<ArticleViewModel>> GetAllArticlesByUserNameAsync(string userName)
    {
        ApplicationUser user = await _context.Users.FirstAsync(u => u.UserName == userName);


        return await _context.Articles
            .Where(a => a.ApplicationUserId == user.Id && a.IsDeleted == false)
            .Select(a => new ArticleViewModel()
            {
                Id = a.Id,
                Title = a.Title,
                CreatedOn = a.CreatedOn,
                EditedOn = a.EditedOn,
                Content = a.Content,
                ApplicationUserName = a.ApplicationUserId
            })
            .OrderByDescending(a => a.CreatedOn)
            .ToListAsync();
    }

    public async Task DeleteArticleAsync(ArticleViewModel model)
    {
        Article article = await _context.Articles.FirstAsync(a => a.Id == model.Id);

        article.IsDeleted = true;
        await _context.SaveChangesAsync();
    }
}

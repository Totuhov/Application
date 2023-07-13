
using Application.Web.ViewModels.Article;

namespace Application.Services.Interfaces;

public interface IBlogService
{
    Task CreatePostAsync(CreateArticleViewModel model);
    Task SavePostAsync(CreateArticleViewModel model);
    Task<CreateArticleViewModel> GetCreateArticleViewModelByIdAsync(string articleId, string userId);
    Task<ArticleViewModel> GetArticleViewModelByIdAsync(string id);
    Task<List<ArticleViewModel>> GetAllArticlesByUserNameAsync(string userName);
    Task DeleteArticleAsync(ArticleViewModel model);
    bool IsUserOwnerOfArticle(string articleId, string userId);
    Task<string> GetUsernameByArticleIdAsync(string articleId);
}


namespace Application.Web.ViewModels.Article;

using Application.Data.Models;
using Application.Services.Mapping;

public class AllArticlesViewModel : IMapFrom<Article>
{
    public string? UserName { get; set; }

    public List<ArticleViewModel> Articles { get; set; } = new List<ArticleViewModel>();
}

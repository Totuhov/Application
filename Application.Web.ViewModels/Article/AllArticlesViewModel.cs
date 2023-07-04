
namespace Application.Web.ViewModels.Article;

public class AllArticlesViewModel
{
    public string? UserName { get; set; }

    public List<ArticleViewModel> Articles { get; set; } = new List<ArticleViewModel>();
}

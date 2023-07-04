
namespace Application.Web.ViewModels.Article;


public class ArticleViewModel
{
    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public DateTime EditedOn { get; set; }

    public string Content { get; set; } = null!;

    public string ApplicationUserName { get; set; } = null!;
}

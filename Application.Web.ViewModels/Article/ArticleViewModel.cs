
namespace Application.Web.ViewModels.Article;

using Data.Models;
using Services.Mapping;

public class ArticleViewModel : IMapFrom<Article>
{
    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public DateTime EditedOn { get; set; }

    public string Content { get; set; } = null!;

    public string ApplicationUserName { get; set; } = null!;

    public bool IsDeleted { get; set; }
}

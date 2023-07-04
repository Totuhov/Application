
using System.ComponentModel.DataAnnotations;

namespace Application.Web.ViewModels.Article;

public class CreateArticleViewModel
{
    public string Id { get; set; } = null!;

    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public DateTime EditedOn { get; set; }

    [Required]
    [StringLength(5000, MinimumLength = 150)]
    public string Content { get; set; } = null!;

    [Required]
    public string ApplicationUserId { get; set; } = null!;

    public bool IsDeleted { get; set; } = false!;
}

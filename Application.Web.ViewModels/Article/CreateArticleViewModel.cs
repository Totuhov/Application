
namespace Application.Web.ViewModels.Article;

using System.ComponentModel.DataAnnotations;

using static Common.ModelConstants.ArticleConstants;

public class CreateArticleViewModel
{
    //public string Id { get; set; } = null!;

    [Required]
    [StringLength(TitleMaxLength, MinimumLength = TitleMinLength)]
    public string Title { get; set; } = null!;

    [Required]
    [Display(Name = "Edited on")]
    public DateTime EditedOn { get; set; }

    [Required]
    [StringLength(ContentMaxLength, MinimumLength = ContentMinLength)]
    public string Content { get; set; } = null!;

    [Required]
    public string ApplicationUserId { get; set; } = null!;

    public bool IsDeleted { get; set; } = false!;
}

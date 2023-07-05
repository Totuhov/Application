
namespace Application.Data.Models;

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using static Application.Common.ModelConstants.ArticleConstants;

public class Article
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [MaxLength(TitleMaxLength)]    
    public string Title { get; set; } = null!;

    [Required]    
    public DateTime CreatedOn { get; set; } = DateTime.Now;

    [Required]
    public DateTime EditedOn { get; set; }

    [Required]
    [MaxLength(ContentMaxLength)]
    public string Content { get; set; } = null!;

    [Required]
    public string ApplicationUserId { get; set; } = null!;

    [ForeignKey(nameof(ApplicationUserId))]
    public virtual ApplicationUser ApplicationUser { get; set; } = null!;

    [Required]
    public bool IsDeleted { get; set; } = false;
}

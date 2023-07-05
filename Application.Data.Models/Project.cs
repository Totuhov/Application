
namespace Application.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using static Application.Common.ModelConstants.ProjectConstants;

public class Project
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [MaxLength(NameMaxLength)]
    public string? Name { get; set; }

    public string? ImageId { get; set; }

    public virtual Image? Image { get; set; }

    [MaxLength(DescriptionMaxLength)]
    public string? Description { get; set; }

    [MaxLength(UrlMaxLength)]
    public string? Url { get; set; }

    public string ApplicationUserId { get; set; } = null!;

    [ForeignKey(nameof(ApplicationUserId))]
    public virtual ApplicationUser ApplicationUser { get; set; } = null!;
}

namespace Application.Web.ViewModels.Project;

using System.ComponentModel.DataAnnotations;

using static Application.Common.ModelConstants.ProjectConstants;

public class CreateProjectViewModel
{
    [Required]
    [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
    public string Name { get; set; } = null!;

    [MaxLength(DescriptionMaxLength)]
    public string? Description { get; set; }

    [MaxLength(UrlMaxLength)]
    public string? Url { get; set; }

    public string ApplicationUserId { get; set; } = null!;
}

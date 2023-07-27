
using System.ComponentModel.DataAnnotations;
using static Application.Common.ModelConstants.ProjectConstants;

namespace Application.Web.ViewModels.Project;

public class EditProjectViewModel
{
    public string Id { get; set; } = null!;

    [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
    public string? Name { get; set; }

    [MaxLength(DescriptionMaxLength)]
    public string? Description { get; set; }

    [MaxLength(UrlMaxLength)]
    public string? Url { get; set; }

    public string ApplicationUserId { get; set; } = null!;
}

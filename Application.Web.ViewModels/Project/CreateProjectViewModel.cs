
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Web.ViewModels.Project;

public class CreateProjectViewModel
{
    [Required]
    [StringLength(50, MinimumLength =2)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Url { get; set; }

    public string ApplicationUserId { get; set; } = null!;
}

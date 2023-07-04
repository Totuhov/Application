
using Application.Web.ViewModels.Image;

namespace Application.Web.ViewModels.Project;

public class ProjectViewModel
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public string? ImageId { get; set; }

    public virtual ImageViewModel? Image { get; set; }

    public string? Description { get; set; }

    public string? Url { get; set; }

    public string ApplicationUserId { get; set; } = null!;
}

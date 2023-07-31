
namespace Application.Web.ViewModels.Project;

using Application.Web.ViewModels.Image;

public class ProjectViewModel
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public string ImageId { get; set; } = null!;

    public virtual ImageViewModel Image { get; set; } = null!;

    public string? Description { get; set; }

    public string? Url { get; set; }

    public string ApplicationUserId { get; set; } = null!;

    public string UserName { get; set; } = null!;
}

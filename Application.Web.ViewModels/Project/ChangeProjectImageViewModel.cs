
namespace Application.Web.ViewModels.Project;

using Application.Web.ViewModels.Image;
public class ChangeProjectImageViewModel
{
    public string ProjectId { get; set; } = null!;

    public string? ImageId { get; set; }

    public List<ImageViewModel>? Images { get; set; }
}

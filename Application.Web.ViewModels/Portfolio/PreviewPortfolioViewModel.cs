
namespace Application.Web.ViewModels.Portfolio;

using Application.Web.ViewModels.Image;

public class PreviewPortfolioViewModel
{
    public ImageViewModel ProfileImage { get; set; } = null!;

    public string UserName { get; set; } = null!;
}

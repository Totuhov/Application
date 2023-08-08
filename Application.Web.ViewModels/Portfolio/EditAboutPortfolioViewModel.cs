
namespace Application.Web.ViewModels.Portfolio;

using System.ComponentModel.DataAnnotations;

using static Application.Common.ModelConstants.PortfolioConstants;

public class EditAboutPortfolioViewModel
{
    [MaxLength(AboutMaxLength)]
    public string? About { get; set; }
}

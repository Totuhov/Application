
namespace Application.Web.ViewModels.Portfolio;

using System.ComponentModel.DataAnnotations;

using static Application.Common.ModelConstants.PortfolioConstants;

public class EditDescriptionPortfolioViewModel
{
    [MaxLength(GreetingsMessageMaxLength)]
    public string? GreetingsMessage { get; set; }

    [MaxLength(UserDisplayNameMaxLength)]
    public string? UserDisplayName { get; set; }

    [MaxLength(DescriptionMaxLength)]
    public string? Description { get; set; }
}

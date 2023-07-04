
using System.ComponentModel.DataAnnotations;

namespace Application.Web.ViewModels.Portfolio;

public class EditDescriptionPortfolioViewModelViewModel
{
    [MaxLength(150)]
    public string? GreetingsMessage { get; set; }

    [MaxLength(100)]
    public string? UserDisplayName { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }
}

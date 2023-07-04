
namespace Application.Web.ViewModels.Portfolio;

using System.ComponentModel.DataAnnotations;

public class EditAboutPortfolioViewModelViewModel
{
    [MaxLength(1000)]
    public string? About { get; set; }
}

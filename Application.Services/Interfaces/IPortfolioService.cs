
using Application.Web.ViewModels.Portfolio;

namespace Application.Services.Interfaces;

public interface IPortfolioService
{
    Task<bool> LogedInUserHasPortfolio(string userId);
    Task CreateFirstPortfolioAsync(string userId);
    Task<EditDescriptionPortfolioViewModelViewModel> GetEditDescriptionViewModelAsync(string userId);
    Task<EditAboutPortfolioViewModelViewModel> GetEditAboutViewModelAsync(string userId);
    Task SaveDescriptionAsync(EditDescriptionPortfolioViewModelViewModel model, string userId);
    Task SaveAboutAsync(EditAboutPortfolioViewModelViewModel model, string userId);
    Task<PortfolioViewModel> GetPortfolioFromRouteAsync(string userName);
    Task<List<PreviewPortfolioViewModel>> GetAllUsersByRegexAsync(string expression);
}

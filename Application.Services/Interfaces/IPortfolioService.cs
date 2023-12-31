﻿
using Application.Web.ViewModels.Portfolio;

namespace Application.Services.Interfaces;

public interface IPortfolioService
{
    Task<bool> LogedInUserHasPortfolio(string userId);
    Task CreateFirstPortfolioAsync(string userId);
    Task<EditDescriptionPortfolioViewModel> GetEditDescriptionViewModelAsync(string userId);
    Task<EditAboutPortfolioViewModel> GetEditAboutViewModelAsync(string userId);
    Task SaveDescriptionAsync(EditDescriptionPortfolioViewModel model, string userId);
    Task SaveAboutAsync(EditAboutPortfolioViewModel model, string userId);
    Task<PortfolioViewModel> GetPortfolioFromRouteAsync(string userName);
    Task<List<PreviewPortfolioViewModel>> GetAllUsersByRegexAsync(string expression);
}

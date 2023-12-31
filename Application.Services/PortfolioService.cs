﻿
namespace Application.Services;

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

using Application.Data;
using Application.Data.Models;
using Application.Services.Interfaces;
using Application.Web.ViewModels.Image;
using Application.Web.ViewModels.Article;
using Application.Web.ViewModels.Project;
using Application.Web.ViewModels.Portfolio;
using Application.Web.ViewModels.SocialMedia;

using static Application.Common.ModelConstants.ImageConstants;

public class PortfolioService : IPortfolioService
{

    private readonly ApplicationDbContext _context;

    public PortfolioService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateFirstPortfolioAsync(string userId)
    {
        ApplicationUser user = await _context.Users.FirstAsync(u => u.Id == userId);

        if (user != null)
        {
            user.Portfolio ??= new();
            user.Portfolio.Image = await _context.Images.FirstAsync(i => i.Characteristic == DefaultProfileImageCharacteristic);
            user.SocialMedia = new SocialMedia();

            await _context.SaveChangesAsync();
        }
    }

    public async Task<EditDescriptionPortfolioViewModel> GetEditDescriptionViewModelAsync(string userId)
    {
        Portfolio portfolio = await _context.Portfolios.FirstAsync(p => p.ApplicationUserId == userId);

        EditDescriptionPortfolioViewModel model = new()
        {
            GreetingsMessage = portfolio.GreetingsMessage,
            UserDisplayName = portfolio.UserDisplayName,
            Description = portfolio.Description
        };

        return model;
    }

    public async Task<EditAboutPortfolioViewModel> GetEditAboutViewModelAsync(string userId)
    {
        Portfolio portfolio = await _context.Portfolios.FirstAsync(p => p.ApplicationUserId == userId);

        EditAboutPortfolioViewModel model = new()
        {
            About = portfolio.About
        };

        return model;

    }

    public async Task<PortfolioViewModel> GetPortfolioFromRouteAsync(string userName)
    {
        ApplicationUser user = await _context.Users.FirstAsync(u => u.UserName == userName);
        Portfolio portfolio = await _context.Portfolios.FirstAsync(p => p.ApplicationUserId == user.Id);

        PortfolioViewModel model = new()
        {
            UserName = userName,
            GreetingsMessage = portfolio.GreetingsMessage,
            UserDisplayName = portfolio.UserDisplayName,
            Description = portfolio.Description,
            Email = user.Email,
            ProfileImage = new ImageViewModel()
            {
                ImageId = portfolio.ImageId,
                ImageData = Convert.ToBase64String(portfolio.Image.Bytes),
                ContentType = portfolio.Image.FileExtension
            },
            About = portfolio.About,
            Blog = await _context
            .Articles
            .Where(a => a.ApplicationUserId == user.Id && a.IsDeleted == false)
            .Select(a => new ArticleViewModel()
            {
                Id = a.Id,
                Title = a.Title,
                Content = a.Content,
                CreatedOn = a.CreatedOn,
                EditedOn = a.EditedOn,
                ApplicationUserName = a.ApplicationUser.UserName,
            })
            .OrderByDescending(a => a.EditedOn)
            .Take(4)
            .ToListAsync(),
            Projects = await _context
            .Projects
            .Where(p => p.ApplicationUserId == user.Id)
            .Select(p => new ProjectViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                ImageId = p.ImageId,
                Description = p.Description,
                Url = p.Url,
                ApplicationUserId = p.ApplicationUserId,
                Image = new ImageViewModel()
                {
                    ImageId = p.Image.ImageId,
                    ApplicationUserId = p.Image.ApplicationUserId,
                    ImageData = Convert.ToBase64String(p.Image.Bytes),
                    ContentType = p.Image.FileExtension
                }
            })
            .ToListAsync(),
            SocialMedia = new EditSocialMediasViewModel()
            {
                Id = user.SocialMedia.Id,
                FacebookUrl = user.SocialMedia.FacebookUrl,
                InstagramUrl = user.SocialMedia.InstagramUrl,
                LinkedInUrl = user.SocialMedia.LinkedInUrl,
                TwiterUrl = user.SocialMedia.TwiterUrl,
            }
        };
        return model;

    }
    public async Task<bool> LogedInUserHasPortfolio(string userId)
    {
        ApplicationUser user = await _context.Users.FirstAsync(u => u.Id == userId);

        return user.Portfolio != null;

    }

    public async Task SaveDescriptionAsync(EditDescriptionPortfolioViewModel model, string userId)
    {
        ApplicationUser user = await _context.Users.FirstAsync(u => u.Id == userId);

        if (user.Portfolio != null)
        {
            user.Portfolio.GreetingsMessage = model.GreetingsMessage;
            user.Portfolio.UserDisplayName = model.UserDisplayName;
            user.Portfolio.Description = model.Description;
        }

        await _context.SaveChangesAsync();
    }

    public async Task SaveAboutAsync(EditAboutPortfolioViewModel model, string userId)
    {
        ApplicationUser user = await _context.Users.FirstAsync(u => u.Id == userId);

        if (user.Portfolio != null)
        {
            user.Portfolio.About = model.About;

            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<PreviewPortfolioViewModel>> GetAllUsersByRegexAsync(string expression)
    {
        return await _context.Portfolios
            .Where(p => p.ApplicationUser.UserName.Contains(expression))
            .Select(p => new PreviewPortfolioViewModel()
            {
                UserName = p.ApplicationUser.UserName,
                ProfileImage = new ImageViewModel()
                {
                    ImageId = p.Image.ImageId,
                    ApplicationUserId = p.Image.ApplicationUserId,
                    ImageData = Convert.ToBase64String(p.Image.Bytes),
                    ContentType = p.Image.FileExtension
                }
            })
            .ToListAsync();
    }
}

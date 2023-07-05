
namespace Application.Services;

using Application.Data;
using Application.Data.Models;
using Application.Services.Interfaces;
using Application.Web.ViewModels.Article;
using Application.Web.ViewModels.Image;
using Application.Web.ViewModels.Portfolio;
using Application.Web.ViewModels.Project;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

using static Application.Common.DbContextConstants;

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
            user.Portfolio.Image = await _context.Images.FirstOrDefaultAsync(i => i.Characteristic == DefaultProfileImageCharacteristic);

            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<EditDescriptionPortfolioViewModelViewModel> GetEditDescriptionViewModelAsync(string userId)
    {

        Portfolio? portfolio = await _context.Portfolios.FirstOrDefaultAsync(p => p.ApplicationUserId == userId);

        EditDescriptionPortfolioViewModelViewModel model = new()
        {
            GreetingsMessage = portfolio.GreetingsMessage,
            UserDisplayName = portfolio.UserDisplayName,
            Description = portfolio.Description
        };

        return model;
    }

    public async Task<EditAboutPortfolioViewModelViewModel> GetEditAboutViewModelAsync(string userId)
    {
        Portfolio? portfolio = await _context.Portfolios.FirstOrDefaultAsync(p => p.ApplicationUserId == userId);

        EditAboutPortfolioViewModelViewModel model = new()
        {
            About = portfolio.About
        };

        return model;
    }

    public async Task<PortfolioViewModel> GetPortfolioFromRouteAsync(string userName)
    {
        ApplicationUser user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        Portfolio? portfolio = await _context.Portfolios.FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id);

        if (portfolio != null)
        {
            if (portfolio.Image == null)
            {
                portfolio.Image = await _context.Images.FirstOrDefaultAsync(i => i.Characteristic == DefaultProfileImageCharacteristic);
            }

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
                .Where(a => a.ApplicationUserId == user.Id)
                .Select(a => new ArticleViewModel()
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = a.Content,
                    CreatedOn = a.CreatedOn,
                    EditedOn = a.EditedOn,
                    ApplicationUserName = a.ApplicationUser.UserName,
                })
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
                .ToListAsync()
            };
            return model;
        }
        return null;
    }
    public async Task<bool> LogedInUserHasPortfolio(string userId)
    {
        ApplicationUser user = await _context.Users.FirstAsync(u => u.Id == userId);

        return user.Portfolio != null;

    }

    public async Task SaveDescriptionAsync(EditDescriptionPortfolioViewModelViewModel model, string userId)
    {
        ApplicationUser? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user != null)
        {
            if (user.Portfolio != null)
            {
                user.Portfolio.GreetingsMessage = model.GreetingsMessage;
                user.Portfolio.UserDisplayName = model.UserDisplayName;
                user.Portfolio.Description = model.Description;

                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task SaveAboutAsync(EditAboutPortfolioViewModelViewModel model, string userId)
    {
        ApplicationUser? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user != null)
        {
            if (user.Portfolio != null)
            {
                user.Portfolio.About = model.About;

                await _context.SaveChangesAsync();
            }
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

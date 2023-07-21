using Application.Data;
using Application.Data.Models;
using Application.Services.Interfaces;
using Application.Web.ViewModels.SocialMedia;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class SocialMediaService : ISocialMediaService
    {
        private readonly ApplicationDbContext _context;

        public SocialMediaService(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task<EditSocialMediasViewModel> GetEditModelByIdAsync(string userId)
        {
            SocialMedia media = await _context.SocialMedias.FirstAsync(s => s.ApplicationUserId == userId);

            return new EditSocialMediasViewModel()
            {
                Id = media.Id,
                ApplicationUserId = userId,
                FacebookUrl = media.FacebookUrl,
                LinkedInUrl = media.LinkedInUrl,
                InstagramUrl = media.InstagramUrl,
                TwiterUrl = media.TwiterUrl
            };
        }

        public async Task SaveChangesToModelAsync(EditSocialMediasViewModel model)
        {
            SocialMedia media = await _context.SocialMedias.FirstAsync(s => s.Id == model.Id);

            media.FacebookUrl = model.FacebookUrl;
            media.InstagramUrl = model.InstagramUrl;
            media.LinkedInUrl = model.LinkedInUrl;
            media.TwiterUrl = model.TwiterUrl;

            await _context.SaveChangesAsync();
        }
    }
}

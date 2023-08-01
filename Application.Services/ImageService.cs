
namespace Application.Services;

using Microsoft.EntityFrameworkCore;

using Application.Services.Interfaces;
using Application.Web.ViewModels.Image;
using Application.Data.Models;
using Application.Data;

using static Application.Common.ModelConstants.ImageConstants;

public class ImageService : IImageService
{
    private readonly ApplicationDbContext _context;

    public ImageService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ImageViewModel>> GetUserImagesAsync(string userId)
    {
        return await _context.Images
            .Where(i => i.ApplicationUserId == userId)
            .Select(i => new ImageViewModel()
            {
                ImageId = i.ImageId,
                ImageData = Convert.ToBase64String(i.Bytes),
                ContentType = GetContentType(i.FileExtension),
                ApplicationUserId = i.ApplicationUserId!
            })
            .ToListAsync();
    }

    public async Task SaveImageInDatabaseAsync(CreateImageViewModel model, string userId)
    {
        using var memoryStream = new MemoryStream();
        await model.File.CopyToAsync(memoryStream);
        byte[] imageData = memoryStream.ToArray();

        memoryStream.Close();

        Image image = new()
        {
            Bytes = imageData,
            FileExtension = Path.GetExtension(model.File.FileName),
            Size = imageData.Length,
            ApplicationUserId = userId
        };

        await _context.Images.AddAsync(image);
        _context.SaveChanges();
    }

    public async Task<ImageViewModel> GetImageByIdAsync(string id)
    {
        Image image = await _context.Images.FirstAsync(i => i.ImageId == id);

        var result = new ImageViewModel()
        {
            ImageId = image.ImageId,
            ImageData = Convert.ToBase64String(image.Bytes),
            ContentType = GetContentType(image.FileExtension),

        };
        if (image.ApplicationUserId != null)
        {
            result.ApplicationUserId = image.ApplicationUserId;
        }

        return result;
    }

    public async Task DeleteImageByIdAsync(string id)
    {
        Image image = await _context.Images.FirstAsync(i => i.ImageId == id);

        if (image.Portfolios.Any(p => p.ImageId == id))
        {
            foreach (var portfolio in image.Portfolios.Where(p => p.ImageId == id))
            {
                portfolio.Image = await _context.Images.FirstAsync(i => i.Characteristic == DefaultProfileImageCharacteristic);
            }
        }

        if (image.Projects.Any(p => p.ImageId == id))
        {
            foreach (var project in image.Projects.Where(p => p.ImageId == id))
            {
                project.Image = await _context.Images.FirstAsync(i => i.Characteristic == DefaultProjectImageCharacteristic);
            }
        }

        _context.Images.Remove(image);
        await _context.SaveChangesAsync();
    }

    public async Task UseImageAsProfilAsync(string imageId, string userId)
    {
        Image image = await _context.Images.FirstAsync(i => i.ImageId == imageId);
        Portfolio portfolio = await _context.Portfolios.FirstAsync(p => p.ApplicationUserId == userId);

        portfolio.Image = image;
        await _context.SaveChangesAsync();
    }

    public static string GetContentType(string fileExtension)
    {        
        return fileExtension.ToLower() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            _ => "application/octet-stream",
        };
    }

    string IImageService.GetContentType(string fileExtension)
    {
        return fileExtension.ToLower() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            _ => "application/octet-stream",
        };
    }

}

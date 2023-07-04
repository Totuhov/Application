
namespace Application.Services;

using Application.Data;
using Application.Data.Models;
using Application.Services.Interfaces;

using static Data.Common.DataConstants.ContextConstants;

using Application.Web.ViewModels.Image;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

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
                ApplicationUserId = i.ApplicationUserId
            })
            .ToListAsync();
    }

    static string GetContentType(string fileExtension)
    {
        // Add additional mappings for different file extensions if needed
        return fileExtension.ToLower() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            _ => "application/octet-stream",
        };
    }

    // It is not possible to implement static method in iterface. In this case is needed
    string IImageService.GetContentType(string fileExtension)
    {
        return "";
    }

    public async Task SaveImageInDatabaseAsync(CreateImageViewModel model, string userId)
    {
        // Read the image file as bytes
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

        // TODO: Save the image to the database using EF Core
        await _context.Images.AddAsync(image);

        _context.SaveChanges();
    }

    public async Task<ImageViewModel> GetImageByIdAsync(string id)
    {
        Image? image = await _context.Images.FindAsync(id);

        if (image != null)
        {
            return new ImageViewModel()
            {
                ImageId = image.ImageId,
                ImageData = Convert.ToBase64String(image.Bytes),
                ContentType = GetContentType(image.FileExtension),
                ApplicationUserId = image.ApplicationUserId
            };
        }
        else
        {
            return new ImageViewModel();
        }
    }

    public async Task DeleteImageByIdAsync(string id)
    {
        Image? image = await _context.Images.FindAsync(id);

        if (image != null)
        {
            if (image.Portfolios.Any(p => p.ImageId == id))
            {
                foreach (var portfolio in image.Portfolios.Where(p => p.ImageId == id))
                {
                    portfolio.Image = await _context.Images.FirstOrDefaultAsync(i => i.Characteristic == DefaultProfileImageCharacteristic);
                }
            }

            if (image.Projects.Any(p => p.ImageId == id))
            {
                foreach (var project in image.Projects.Where(p => p.ImageId == id))
                {
                    project.Image = await _context.Images.FirstOrDefaultAsync(i => i.Characteristic == DefaultProjectImageCharacteristic);
                }
            }
            _context.Images.Remove(image);
            await _context.SaveChangesAsync();
        }
    }
    public async Task UseImageAsProfilAsync(string imageId, string userId)
    {
        ApplicationUser? user = await _context.Users.FindAsync(userId);
        Image? image = await _context.Images.FindAsync(imageId);

        if (user != null && image != null)
        {
            if (user.Portfolio != null)
            {
                user.Portfolio.Image = image;
                await _context.SaveChangesAsync();
            }
        }
    }
}

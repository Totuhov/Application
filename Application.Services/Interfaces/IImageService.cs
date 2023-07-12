
namespace Application.Services.Interfaces;

using Application.Web.ViewModels.Image;

public interface IImageService
{
    Task<List<ImageViewModel>> GetUserImagesAsync(string userId);
    string GetContentType(string fileExtension);
    Task SaveImageInDatabaseAsync(CreateImageViewModel model, string userId);
    Task<ImageViewModel> GetImageByIdAsync(string id);
    Task DeleteImageByIdAsync(string id);
    Task UseImageAsProfilAsync(string imageId, string userId);
}

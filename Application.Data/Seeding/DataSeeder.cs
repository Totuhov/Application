
namespace Application.Data.Seeding;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

using Application.Data.Models;

using static Application.Common.DbContextConstants;

public class DataSeeder
{
    private readonly string _profileImageFolderPath;
    private readonly string _projectImageFolderPath;

    public DataSeeder(IWebHostEnvironment env)
    {
        _profileImageFolderPath = Path.Combine(env.WebRootPath, "images", "profile");
        _projectImageFolderPath = Path.Combine(env.WebRootPath, "images", "project");
    }

    public void Seed(ModelBuilder modelBuilder)
    {

        // Seed images
        SeedImages(modelBuilder);
    }

    private void SeedImages(ModelBuilder modelBuilder)
    {
        // Read the image files from the root folder
        string[] profileImageFiles = Directory.GetFiles(_profileImageFolderPath);
        string[] projectImageFiles = Directory.GetFiles(_projectImageFolderPath);


        foreach (string imagePath in profileImageFiles)
        {

            // Convert image file to byte array
            byte[] imageData = File.ReadAllBytes(imagePath);

            // Create an Image entity and add it to the model
            modelBuilder.Entity<Image>().HasData(new Image
            {
                Bytes = imageData,
                FileExtension = Path.GetExtension(imagePath),
                Size = imageData.Length,
                Characteristic = DefaultProfileImageCharacteristic
            });
        }
        foreach (string imagePath in projectImageFiles)
        {

            // Convert image file to byte array
            byte[] imageData = File.ReadAllBytes(imagePath);

            // Create an Image entity and add it to the model
            modelBuilder.Entity<Image>().HasData(new Image
            {
                Bytes = imageData,
                FileExtension = Path.GetExtension(imagePath),
                Size = imageData.Length,
                Characteristic = DefaultProjectImageCharacteristic
            });
        }
    }    
}

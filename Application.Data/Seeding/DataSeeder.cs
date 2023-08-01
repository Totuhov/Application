
namespace Application.Data.Seeding;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using Application.Data.Models;

using static Application.Common.ModelConstants.ImageConstants;

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
        SeedImages(modelBuilder);
        SeedRoles(modelBuilder);
        SeedUsers(modelBuilder);
    }

    private static void SeedUsers(ModelBuilder modelBuilder)
    {
        var hasher = new PasswordHasher<IdentityUser>();

        modelBuilder.Entity<ApplicationUser>().HasData(
            new IdentityUser
            {
                Id = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                UserName = "Administrator",
                NormalizedUserName = "ADMINISTRATOR",
                PasswordHash = hasher.HashPassword(null, "Aa1234!")
            }
        );

        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",
                UserId = "8e445865-a24d-4543-a6c6-9443d048cdb9"
            }
        );
    }

    private static void SeedRoles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityRole>().
            HasData(new IdentityRole { Id = "2c5e174e-3b0e-446f-86af-483d56fd7210", Name = "Admin", NormalizedName = "ADMIN".ToUpper() });

        modelBuilder.Entity<IdentityRole>().
            HasData(new IdentityRole { Id = "2c5e174e-3b0e-446f-86af-483d56fd7211", Name = "User", NormalizedName = "USER".ToUpper() });
    }

    private void SeedImages(ModelBuilder modelBuilder)
    {

        // Read the image files from the root folder
        string[] profileImageFiles = Directory.GetFiles(_profileImageFolderPath);
        string[] projectImageFiles = Directory.GetFiles(_projectImageFolderPath);


        foreach (string imagePath in profileImageFiles)
        {

            byte[] imageData = File.ReadAllBytes(imagePath);

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
            byte[] imageData = File.ReadAllBytes(imagePath);

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

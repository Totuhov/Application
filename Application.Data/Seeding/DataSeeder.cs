
namespace Application.Data.Seeding;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

using Application.Data.Models;

using static Application.Common.ModelConstants;
using System;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Emit;

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
        SeedRoles(modelBuilder);
        SeedUsers(modelBuilder);
    }

    private void SeedUsers(ModelBuilder modelBuilder)
    {
        var hasher = new PasswordHasher<IdentityUser>();


        //Seeding the User to AspNetUsers table
        modelBuilder.Entity<ApplicationUser>().HasData(
            new IdentityUser
            {
                Id = "8e445865-a24d-4543-a6c6-9443d048cdb9", // primary key
                UserName = "Administrator",
                NormalizedUserName = "ADMINISTRATOR",
                PasswordHash = hasher.HashPassword(null, "Aa1234!")
            }
        );


        //Seeding the relation between our user and role to AspNetUserRoles table
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",
                UserId = "8e445865-a24d-4543-a6c6-9443d048cdb9"
            }
        );
    }

    private void SeedRoles(ModelBuilder modelBuilder)
    {
        //Seeding a  'Administrator' role to AspNetRoles table
        modelBuilder.Entity<IdentityRole>().
            HasData(new IdentityRole { Id = "2c5e174e-3b0e-446f-86af-483d56fd7210", Name = "Admin", NormalizedName = "ADMIN".ToUpper() });

        //Seeding a  'Administrator' role to AspNetRoles table
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

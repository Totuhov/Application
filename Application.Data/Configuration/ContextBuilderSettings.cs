using Application.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Data.Configuration;

public class ContextBuilderSettings
{
    public static void SeEntityOptions(ModelBuilder builder)
    {
        builder.Entity<ApplicationUser>()
            .HasOne(a => a.Portfolio)
            .WithOne(a => a.ApplicationUser)
            .HasForeignKey<Portfolio>(a => a.ApplicationUserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplicationUser>()
            .HasMany(e => e.Images)
            .WithOne(e => e.ApplicationUser)
            .HasForeignKey(e => e.ApplicationUserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplicationUser>()
            .HasMany(e => e.Articles)
            .WithOne(e => e.ApplicationUser)
            .HasForeignKey(e => e.ApplicationUserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplicationUser>()
            .HasMany(e => e.Projects)
            .WithOne(e => e.ApplicationUser)
            .HasForeignKey(e => e.ApplicationUserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplicationUser>()
            .HasOne(e => e.SocialMedia)
            .WithOne(e => e.ApplicationUser)
            .HasForeignKey<SocialMedia>(e => e.ApplicationUserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Image>()
            .HasMany(e => e.Projects)
            .WithOne(e => e.Image)
            .HasForeignKey(e => e.ImageId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);


        builder.Entity<Image>()
            .HasMany(e => e.Portfolios)
            .WithOne(e => e.Image)
            .HasForeignKey(e => e.ImageId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);

        //var seeder = new DataSeeder(_env);
        //seeder.Seed(builder);
    }
}

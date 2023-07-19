
using Application.Data.Models;
using Application.Data.Seeding;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly IWebHostEnvironment _env;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IWebHostEnvironment env)
        : base(options)
    {
        _env = env;
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Portfolio> Portfolios { get; set; } = null!;

    public DbSet<Image> Images { get; set; } = null!;

    public DbSet<Project> Projects { get; set; } = null!;

    public DbSet<Article> Articles { get; set; } = null!;



    protected override void OnModelCreating(ModelBuilder builder)
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

        var seeder = new DataSeeder(_env);
        seeder.Seed(builder);

        base.OnModelCreating(builder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies(); // Enable lazy loading        
    }

}

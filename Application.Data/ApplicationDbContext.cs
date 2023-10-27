using Application.Data.Configuration;
using Application.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly IWebHostEnvironment _env = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IWebHostEnvironment env)
        : base(options)
    {
        _env = env;
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {

    }

    public DbSet<Portfolio> Portfolios { get; set; } = null!;

    public DbSet<Image> Images { get; set; } = null!;

    public DbSet<Project> Projects { get; set; } = null!;

    public DbSet<Article> Articles { get; set; } = null!;

    public DbSet<SocialMedia> SocialMedias { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {

        ContextBuilderSettings.SeEntityOptions(builder);

        //var seeder = new DataSeeder(_env);
        //seeder.Seed(builder);

        base.OnModelCreating(builder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }

}

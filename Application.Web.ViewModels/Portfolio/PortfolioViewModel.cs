﻿
namespace Application.Web.ViewModels.Portfolio;

using System.ComponentModel.DataAnnotations;

using Application.Web.ViewModels.Article;
using Application.Web.ViewModels.ContactForm;
using Application.Web.ViewModels.Image;
using Application.Web.ViewModels.Project;
using Application.Web.ViewModels.SocialMedia;

public class PortfolioViewModel
{
    public string Id { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? GreetingsMessage { get; set; }
        
    public string? UserDisplayName { get; set; }

    public string? Description { get; set; }

    public string? About { get; set; }

    public ImageViewModel ProfileImage { get; set; } = null!;
        
    public ICollection<ProjectViewModel> Projects { get; set; } = new List<ProjectViewModel>();

    public ICollection<ArticleViewModel> Blog { get; set; } = new List<ArticleViewModel>();

    public ContactFormViewModel? ContactForm { get; set; } = new();

    public EditSocialMediasViewModel? SocialMedia { get; set; } = new();
}

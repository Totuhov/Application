
namespace Application.Web.ViewModels.SocialMedia;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using static Application.Common.ModelConstants.SocialMediaConstants;

public class EditSocialMediasViewModel
{
    public string Id { get; set; } = null!;

    [MaxLength(UrlMaxLength)]
    [DisplayName(FacebookDisplayName)]
    public string? FacebookUrl { get; set; }

    [MaxLength(UrlMaxLength)]
    [DisplayName(InstagramDisplayName)]
    public string? InstagramUrl { get; set; }

    [MaxLength(UrlMaxLength)]
    [DisplayName(LinkedInDisplayName)]
    public string? LinkedInUrl { get; set; }

    [MaxLength(UrlMaxLength)]
    [DisplayName(TwitterDisplayName)]
    public string? TwiterUrl { get; set; }

    [Required]
    public string ApplicationUserId { get; set; } = null!;
}

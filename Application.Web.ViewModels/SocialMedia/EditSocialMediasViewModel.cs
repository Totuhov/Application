
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Web.ViewModels.SocialMedia
{
    public class EditSocialMediasViewModel
    {
        public string Id { get; set; } = null!;

        [MaxLength(2048)]
        [DisplayName("Facebook")]
        public string? FacebookUrl { get; set; }

        [MaxLength(2048)]
        [DisplayName("Instagram")]
        public string? InstagramUrl { get; set; }

        [MaxLength(2048)]
        [DisplayName("LinkedIn")]
        public string? LinkedInUrl { get; set; }

        [MaxLength(2048)]
        [DisplayName("Twitter")]
        public string? TwiterUrl { get; set; }

        [Required]
        public string ApplicationUserId { get; set; } = null!;
    }
}

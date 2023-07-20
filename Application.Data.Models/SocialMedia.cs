using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Data.Models
{
    public class SocialMedia
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [MaxLength(2048)]
        public string? FacebookUrl { get; set; }

        [MaxLength(2048)]
        public string? InstagramUrl { get; set; }

        [MaxLength(2048)]
        public string? LinkedInUrl { get; set; }

        [MaxLength(2048)]
        public string? TwiterUrl { get; set; }

        public string ApplicationUserId { get; set; } = null!;

        [ForeignKey(nameof(ApplicationUserId))]
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Data.Models
{
    public class SocialMedia
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public Portfolio Portfolio { get; set; } = null!;

        [MaxLength(2048)]
        public string? FacebookUrl { get; set; }

        [MaxLength(2048)]
        public string? InstagramUrl { get; set; }

        [MaxLength(2048)]
        public string? LinkedInUrl { get; set; }

        [MaxLength(2048)]
        public string? TwiterUrl { get; set; }
    }
}

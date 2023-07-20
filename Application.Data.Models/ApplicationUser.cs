
namespace Application.Data.Models;

using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public virtual Portfolio? Portfolio { get; set; }

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<Article> Articles { get; set;} = new List<Article>();

    public virtual SocialMedia SocialMedia { get; set; } = null!;
}

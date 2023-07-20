
namespace Application.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using static Application.Common.ModelConstants.PortfolioConstants;

public class Portfolio
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [MaxLength(GreetingsMessageMaxLength)]
    public string? GreetingsMessage { get; set; } = GreetingsMessageDefaultText;

    [MaxLength(UserDisplayNameMaxLength)]
    public string? UserDisplayName { get; set; } = UserDisplayNameDefaultText;

    [MaxLength(DescriptionMaxLength)]
    public string? Description { get; set; } = DescriptionDefaultText;

    [MaxLength(AboutMaxLength)]
    public string? About { get; set; } = AboutDefaultText;

    public string ImageId { get; set; } = null!;

    [ForeignKey(nameof(ImageId))]
    public virtual Image Image { get; set; } = null!;

    public string ApplicationUserId { get; set; } = null!;

    [ForeignKey(nameof(ApplicationUserId))]
    public virtual ApplicationUser ApplicationUser { get; set; } = null!;
}
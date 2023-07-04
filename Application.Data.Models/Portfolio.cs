
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Data.Models;

public class Portfolio
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string? GreetingsMessage { get; set; } = "Hallo ";

    public string? UserDisplayName { get; set; } = "friend";

    public string? Description { get; set; } = "Hier you can write somethig to describe you...";

    public string? About { get; set; } = "here you can describe your work, competences or just a short autobiography";

    public string? ImageId { get; set; }

    [ForeignKey(nameof(ImageId))]
    public virtual Image? Image { get; set; }

    public string ApplicationUserId { get; set; } = null!;

    public virtual ApplicationUser ApplicationUser { get; set; } = null!;

}
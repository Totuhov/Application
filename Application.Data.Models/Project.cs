
namespace Application.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Project
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [MaxLength(100)]
    public string? Name { get; set; }

    public string? ImageId { get; set; }

    public virtual Image? Image { get; set; }

    public string? Description { get; set; }

    public string? Url { get; set; }

    public string ApplicationUserId { get; set; } = null!;

    [ForeignKey(nameof(ApplicationUserId))]
    public virtual ApplicationUser ApplicationUser { get; set; } = null!;
}
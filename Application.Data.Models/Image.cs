
namespace Application.Data.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

using static Application.Common.ModelConstants.ImageConstants;

public class Image
{
    [Key]
    public string ImageId { get; set; } = Guid.NewGuid().ToString();
        
    public byte[] Bytes { get; set; } = null!;

    [Required]
    [MaxLength(FileExtensionMaxLength)]
    public string FileExtension { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Size { get; set; }

    [FromForm]
    [NotMapped]        
    public IFormFile File { get; set; } = null!;    

    public string? ApplicationUserId { get; set; }

    [ForeignKey(nameof(ApplicationUserId))]
    public virtual ApplicationUser? ApplicationUser { get; set; }

    [MaxLength(CharacteristicMaxLength)]   
    public string? Characteristic { get; set; }

    public virtual ICollection<Portfolio> Portfolios { get; set; } = new List<Portfolio>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
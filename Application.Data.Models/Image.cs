
namespace Application.Data.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

public class Image
{
    [Key]
    public string ImageId { get; set; } = Guid.NewGuid().ToString();
        
    public byte[] Bytes { get; set; } = null!;

    public string FileExtension { get; set; } = null!;

    public decimal Size { get; set; }

    [FromForm]
    [NotMapped]        
    public IFormFile File { get; set; } = null!;    

    public string? ApplicationUserId { get; set; }

    [ForeignKey(nameof(ApplicationUserId))]
    public virtual ApplicationUser? ApplicationUser { get; set; }

    [MaxLength(20)]   
    public string? Characteristic { get; set; }

    public virtual ICollection<Portfolio> Portfolios { get; set; } = new List<Portfolio>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
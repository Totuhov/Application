
namespace Application.Web.ViewModels.Image;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

public class CreateImageViewModel
{
    public byte[] Bytes { get; set; } = null!;

    public decimal Size { get; set; }

    [FromForm]
    [NotMapped]
    public IFormFile File { get; set; } = null!;
}

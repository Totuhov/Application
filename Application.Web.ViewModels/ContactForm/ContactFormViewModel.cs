
namespace Application.Web.ViewModels.ContactForm;

using System.ComponentModel.DataAnnotations;

public class ContactFormViewModel
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string SenderName { get; set; } = null!;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string SenderEmail { get; set; } = null!;

    [Required]
    [StringLength (5000, MinimumLength = 20)]
    public string Text { get; set; } = null!;

    public string RecieverEmail { get; set; } = null!;

    public string RecieverUserName = null!;
}

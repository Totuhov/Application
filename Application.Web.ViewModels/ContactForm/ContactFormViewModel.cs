
namespace Application.Web.ViewModels.ContactForm;

using System.ComponentModel.DataAnnotations;

using static Application.Common.ModelConstants.ContactFormConstants;

public class ContactFormViewModel
{
    [Required]
    [StringLength(SenderNameMaxLength, MinimumLength = SenderNameMinLength)]
    public string SenderName { get; set; } = null!;

    [Required]
    [EmailAddress]
    [MaxLength(SenderEmailMaxLength)]
    public string SenderEmail { get; set; } = null!;

    [Required]
    [MaxLength (TextMaxLength)]
    public string Text { get; set; } = null!;

    public string RecieverEmail { get; set; } = null!;

    public string RecieverUserName = null!;
}

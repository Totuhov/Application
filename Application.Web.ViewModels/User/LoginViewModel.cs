
using System.ComponentModel.DataAnnotations;

namespace Application.Web.ViewModels.User;

public class LoginViewModel
{
    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "User Name")]
    public string UserName { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}

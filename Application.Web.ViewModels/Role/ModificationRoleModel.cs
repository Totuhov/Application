
using System.ComponentModel.DataAnnotations;

namespace Application.Web.ViewModels.Role;

public class ModificationRoleModel
{
    [Required]
    public string RoleName { get; set; }

    public string RoleId { get; set; }

    public string[]? AddIds { get; set; }

    public string[]? DeleteIds { get; set; }
}

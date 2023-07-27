
namespace Application.Web.ViewModels.Role;

using System.ComponentModel.DataAnnotations;

using static Application.Common.ModelConstants.RoleConstants;

public class ModificationRoleModel
{
    [Required]
    [StringLength(RoleNameMaxLength, MinimumLength = RoleNameMinLength)]
    public string RoleName { get; set; } = null!;

    public string RoleId { get; set; } = null!;

    public string[]? AddIds { get; set; }

    public string[]? DeleteIds { get; set; }
}

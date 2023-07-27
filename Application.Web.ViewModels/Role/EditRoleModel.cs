
namespace Application.Web.ViewModels.Role;

using Application.Data.Models;
using Microsoft.AspNetCore.Identity;

public class EditRoleModel
{
    public IdentityRole Role { get; set; } = null!;
    public IEnumerable<ApplicationUser> Members { get; set; } = null!;
    public IEnumerable<ApplicationUser> NonMembers { get; set; } = null!; 
}

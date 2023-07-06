
namespace Application.Web.ViewModels.Role;

using Application.Data.Models;
using Microsoft.AspNetCore.Identity;

public class EditRoleModel
{
    public IdentityRole Role { get; set; }
    public IEnumerable<ApplicationUser> Members { get; set; }
    public IEnumerable<ApplicationUser> NonMembers { get; set; }
}

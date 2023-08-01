
namespace Application.Web.Areas.Admin.Controllers;

using Application.Data.Models;
using Application.Web.Controllers;
using Application.Web.ViewModels.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

using static Application.Common.NotificationMessagesConstants;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class RolesController : BaseController
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RolesController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    /// <summary>
    /// HttpGet Index
    /// </summary>
    /// <returns>View(IQueryable from all current roles in the application)</returns>
    public ViewResult Index() => View(_roleManager.Roles);

    /// <summary>
    /// HttpGet Create Role 
    /// </summary>
    /// <returns>View()</returns>
    public IActionResult Create() => View();

    /// <summary>
    /// HttpPost Create Role 
    /// </summary>
    /// <param name="name"></param>
    /// <returns>RedirectToAction("Index") if result.Succeeded else View(name)</returns>
    [HttpPost]
    public async Task<IActionResult> Create([Required] string name)
    {
        if (await _roleManager.RoleExistsAsync(name))
        {
            this.TempData[ErrorMessage] = $"Role with name {name} allready exists.";
            return RedirectToAction("Index");
        }
        if (ModelState.IsValid)
        {

            IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));
            if (result.Succeeded)
            {
                this.TempData[SuccessMessage] = $"Role {name} was created successfully.";
                return RedirectToAction("Index");
            }
            else
                Errors(result);
        }
        return View(name);
    }

    /// <summary>
    /// HttpPost Delete Role
    /// </summary>
    /// <param name="id"></param>
    /// <returns>RedirectToAction("Index") if result.Succeeded else View("Index", _roleManager.Roles) with ModelState Error "No role found"</returns>
    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        IdentityRole role = await _roleManager.FindByIdAsync(id);
        if (role != null)
        {
            IdentityResult result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
                return RedirectToAction("Index");
            else
                Errors(result);
        }
        else
            ModelState.AddModelError("", "No role found");
        return View("Index", _roleManager.Roles);
    }

    /// <summary>
    /// HttpGet Update Role
    /// </summary>
    /// <param name="id"></param>
    /// <returns>View(new EditRoleModel)</returns>
    public async Task<IActionResult> Update(string id)
    {
        IdentityRole role = await _roleManager.FindByIdAsync(id);
        List<ApplicationUser> members = new();
        List<ApplicationUser> nonMembers = new();
        foreach (ApplicationUser user in _userManager.Users)
        {
            var list = await _userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
            list.Add(user);
        }
        return View(new EditRoleModel
        {
            Role = role,
            Members = members,
            NonMembers = nonMembers
        });
    }

    /// <summary>
    /// HttpPost Update Role
    /// </summary>
    /// <param name="model"></param>
    /// <returns>If ModelState.IsValid - RedirectToAction(nameof(Index) else - Update(model.RoleId)</returns>
    [HttpPost]
    public async Task<IActionResult> Update(ModificationRoleModel model)
    {
        IdentityResult result;
        if (ModelState.IsValid)
        {
            foreach (string userId in model.AddIds ?? Array.Empty<string>())
            {
                ApplicationUser user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    result = await _userManager.AddToRoleAsync(user, model.RoleName);
                    if (!result.Succeeded)
                        Errors(result);
                }
            }
            foreach (string userId in model.DeleteIds ?? Array.Empty<string>())
            {
                ApplicationUser user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);
                    if (!result.Succeeded)
                        Errors(result);
                }
            }
        }

        if (ModelState.IsValid)
            return RedirectToAction(nameof(Index));
        else
            return await Update(model.RoleId);
    }

    /// <summary>
    /// Fill ModelState with Errors
    /// </summary>
    /// <param name="result"></param>
    private void Errors(IdentityResult result)
    {
        foreach (IdentityError error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
    }
}


namespace Application.Services;

using Application.Data;
using Application.Data.Models;
using Application.Services.Interfaces;
using Application.Web.ViewModels.Image;
using Application.Web.ViewModels.Project;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

using static Application.Common.ModelConstants;

public class ProjectService : IProjectService
{
    private readonly ApplicationDbContext _context;

    public ProjectService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddImageToProjectAsync(string projectId, string imageId)
    {
        Project project = await _context.Projects.FirstAsync(p => p.Id == projectId);
        Image image = await _context.Images.FirstAsync(i => i.ImageId == imageId);

        project.Image = image;

        await _context.SaveChangesAsync();
    }

    public async Task<List<ProjectViewModel>> GetAllProjectsFromUserByUsernameAsync(string username)
    {
        ApplicationUser user = await _context.Users.FirstAsync(u => u.UserName == username);

        return await _context
            .Projects
            .Where(p => p.ApplicationUserId == user.Id)
            .Select(p => new ProjectViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Url = p.Url,
                ImageId = p.ImageId,
                Image = new ImageViewModel()
                {
                    ImageId = p.Image.ImageId,
                    ApplicationUserId = p.Image.ApplicationUserId,
                    ImageData = Convert.ToBase64String(p.Image.Bytes),
                    ContentType = p.Image.FileExtension
                },
                ApplicationUserId = p.ApplicationUserId,
            })
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<ProjectViewModel> GetCurrentProjectAsync(string projectId)
    {
        Project project = await _context.Projects.FirstAsync(p => p.Id == projectId);

        return new ProjectViewModel()
        {
            Image = new ImageViewModel()
            {
                ImageId = project.Image.ImageId,
                ApplicationUserId = project.Image.ApplicationUserId,
                ImageData = Convert.ToBase64String(project.Image.Bytes),
                ContentType = project.Image.FileExtension

            },
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Url = project.Url,
        };
    }

    public async Task<EditProjectViewModel> GetEditProjectViewModelAsync(string projectId)
    {
        Project project = await _context.Projects.FirstAsync(p => p.Id == projectId);

        return new EditProjectViewModel()
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Url = project.Url,
        };
    }

    public async Task SaveProjectChangesAsync(EditProjectViewModel model)
    {
        Project project = await _context.Projects.FirstAsync(p => p.Id == model.Id);

        project.Name = model.Name;
        project.Description = model.Description;
        project.Url = model.Url;

        await _context.SaveChangesAsync();
    }

    public async Task SaveProjectForUserAsync(string userId, CreateProjectViewModel model)
    {
        ApplicationUser user = await _context.Users.FirstAsync(u => u.Id == userId);

        Project project = new()
        {
            Name = model.Name,
            Description = model.Description,
            Url = model.Url,
            ApplicationUserId = user.Id,
            Image = await _context.Images
            .FirstAsync(i => i.Characteristic == DefaultProjectImageCharacteristic)
        };

        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProjectByIdAsync(string projectId)
    {
        Project project = await _context.Projects.FirstAsync(p => p.Id == projectId);

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
    }
}

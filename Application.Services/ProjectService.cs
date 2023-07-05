
namespace Application.Services;

using Application.Data;
using Application.Data.Models;
using Application.Services.Interfaces;
using Application.Web.ViewModels.Image;
using Application.Web.ViewModels.Project;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

using static Application.Common.DbContextConstants;

public class ProjectService : IProjectService
{
    private readonly ApplicationDbContext _context;

    public ProjectService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddImageToProjectAsync(string projectId, string imageId)
    {
        Project? project = await _context.Projects.FindAsync(projectId);
        Image? image = await _context.Images.FindAsync(imageId);

        if (project != null && image != null)
        {
            project.Image = image;

            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<ProjectViewModel>> GetAllProjectsFromUserByUsernameAsync(string username)
    {
        ApplicationUser? user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);

        List<ProjectViewModel> result = await _context
            .Projects
            .Where(p => p.ApplicationUserId == user.Id)
            .Select(p => new ProjectViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Url = p.Url,
                ImageId = p.ImageId,
                ApplicationUserId = p.ApplicationUserId,
            })
            .OrderBy(p => p.Name)
            .ToListAsync();

        foreach (var item in result)
        {
            if (item.ImageId != null)
            {
                Image? image = await _context.Images.FindAsync(item.ImageId);

                if (image != null)
                {
                    ImageViewModel imageModel = new()
                    {
                        ImageId = image.ImageId,
                        ApplicationUserId = image.ApplicationUserId,
                        ImageData = Convert.ToBase64String(image.Bytes),
                        ContentType = image.FileExtension
                    };

                    item.Image = imageModel;
                }
            }
        }

        return result;
    }

    public async Task<ProjectViewModel> GetCurrentProjectAsync(string projectId)
    {
        Project? project = await _context.Projects.FindAsync(projectId);

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

    public async Task SaveProjectChangesAsync(ProjectViewModel model)
    {
        Project? project = await _context.Projects.FindAsync(model.Id);

        if (project != null)
        {
            project.Name = model.Name;
            project.Description = model.Description;
            project.Url = model.Url;

            await _context.SaveChangesAsync();
        }
    }

    public async Task SaveProjectForUserAsync(string userId, CreateProjectViewModel model)
    {
        ApplicationUser? user = await _context.Users.FindAsync(userId);

        if (user != null)
        {
            Project project = new()
            {
                Name = model.Name,
                Description = model.Description,
                Url = model.Url,
                ApplicationUserId = user.Id,
                Image = await _context.Images
                .Where(i => i.Characteristic == DefaultProjectImageCharacteristic)
                .FirstOrDefaultAsync()
            };

            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteProjectByIdAsync(string projectId)
    {
        Project? project = await _context.Projects.FindAsync(projectId);
        if (project != null)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }
    }
}

﻿
using Application.Web.ViewModels.Project;

namespace Application.Services.Interfaces;

public interface IProjectService
{
    Task AddImageToProjectAsync(string projectId, string imageId);
    Task<List<ProjectViewModel>> GetAllProjectsFromUserByUsernameAsync(string userName);
    Task<ProjectViewModel> GetCurrentProjectAsync(string projectId);
    Task<EditProjectViewModel> GetEditProjectViewModelAsync(string projectId);
    Task SaveProjectForUserAsync(string userId, CreateProjectViewModel model);
    Task SaveProjectChangesAsync(EditProjectViewModel model);
    Task DeleteProjectByIdAsync(string projectId);
}

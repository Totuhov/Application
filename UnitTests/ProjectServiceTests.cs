﻿
using Application.Data;
using Application.Data.Models;
using Application.Services.Interfaces;
using Application.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Microsoft.AspNetCore.Builder;
using Application.Web.ViewModels.Project;

namespace Application.UnitTests;

[TestFixture]
public class ProjectServiceTests
{
    private ApplicationDbContext _context;

    [OneTimeSetUp]
    public void TestInitialize()
    {
        List<ApplicationUser> users = new()
        {
            new ApplicationUser()
            {
                Id = "1",
                UserName = "guest",
                Portfolio = new()
                {
                    Id = "11",
                    GreetingsMessage = "Greetings message",
                    Description = "Description",
                    UserDisplayName = "User's display name",
                    About = "About user",
                    ApplicationUserId = "1"
                }
            },
            new ApplicationUser()
            {
                Id = "2",
                UserName = "test",
                Portfolio = new()
                {
                    Id = "12",
                    GreetingsMessage = "Greetings message",
                    Description = "Description",
                    UserDisplayName = "User's display name",
                    About = "About user",
                    ImageId = "113",
                    ApplicationUserId = "2"
                }
            },
            new ApplicationUser()
            {
                Id = "3",
                UserName = "TestWithoutPortfolio"
            }
        };

        List<Image> images = new()
        {
            new()
            {
                ImageId = "111",
                ApplicationUserId = null,
                FileExtension = ".png",
                Bytes = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68,
                    82, 0, 0, 2, 0, 0, 0, 2, 0, 8, 2, 0, 0, 0, 123, 26, 67, 173, 0, 0, 0 },
                Characteristic = "defaultProfileImage"
            },
            new()
            {
                ImageId = "112",
                ApplicationUserId = null,
                FileExtension = ".png",
                Bytes = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68,
                    82, 0, 0, 2, 0, 0, 0, 2, 0, 8, 2, 0, 0, 0, 123, 26, 67, 173, 0, 0, 0 },
                Characteristic = "defaultProjectImage"
            },
            new()
            {
                ImageId = "113",
                ApplicationUserId = null,
                FileExtension = ".png",
                Bytes = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68,
                    82, 0, 0, 2, 0, 0, 0, 2, 0, 8, 2, 0, 0, 0, 123, 26, 67, 173, 0, 0, 0 }
            }
        };

        List<Project> projects = new()
        {
            new()
            {
                Id = "1",
                Name = "Test project 1",
                ImageId = "112",
                Description = "Project description",
                Url = null,
                ApplicationUserId = "1",
            },
            new()
            {
                Id = "2",
                Name = "Test project 2",
                ImageId = "112",
                Description = "Project description",
                Url = "https://custom",
                ApplicationUserId = "1",
            }
        };

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryProjectServiceDatabase")
            .Options;

        this._context = new ApplicationDbContext(options);
        this._context.Users.AddRange(users);
        this._context.Projects.AddRange(projects);
        this._context.Images.AddRange(images);
        this._context.SaveChanges();
    }

    [Test]
    [Order(1)]
    public async Task Test_AddImageToProjectAsync_Succeed()
    {
        IProjectService service = new ProjectService(this._context);
        Project project = await this._context.Projects.FirstAsync(p => p.Id == "1");

        await service.AddImageToProjectAsync("1", "113");

        Assert.That(project.Image.ImageId, Is.EqualTo("113"));
    }

    [Test]
    [Order(2)]
    public async Task Test_AddImageToProjectAsync_Fail()
    {
        IProjectService service = new ProjectService(this._context);
        Project project = await this._context.Projects.FirstAsync(p => p.Id == "1");

        await service.AddImageToProjectAsync("1", "115");

        Assert.That(project.Image.ImageId, Is.EqualTo("113"));
    }

    [Test]
    [Order(3)]
    public async Task Test_GetAllProjectsFromUserByUsernameAsync_SucceedWithProjects()
    {
        IProjectService service = new ProjectService(this._context);

        List<ProjectViewModel> projects = await service.GetAllProjectsFromUserByUsernameAsync("guest");

        Assert.That(projects, Has.Count.EqualTo(2));
    }

    [Test]
    [Order(4)]
    public async Task Test_GetAllProjectsFromUserByUsernameAsync_SucceedWithoutProjects()
    {
        IProjectService service = new ProjectService(this._context);

        List<ProjectViewModel> projects = await service.GetAllProjectsFromUserByUsernameAsync("test");

        Assert.That(projects, Is.Empty);
    }

    [Test]
    [Order(5)]
    public async Task Test_GetCurrentProjectAsync_Succeed()
    {
        IProjectService service = new ProjectService(this._context);
        ProjectViewModel? testModel = await service.GetCurrentProjectAsync("1");

        Assert.That(testModel, Is.Not.EqualTo(null));
        Assert.That(testModel.Name, Is.EqualTo("Test project 1"));
    }

    [Test]
    [Order(6)]
    public async Task Test_GetCurrentProjectAsync_Fail()
    {
        IProjectService service = new ProjectService(this._context);

        ProjectViewModel? testModel = await service.GetCurrentProjectAsync("5");

        Assert.That(testModel, Is.EqualTo(null));
    }

    [Test]
    [Order(7)]
    public async Task Test_GetEditProjectViewModelAsync_Succeed()
    {
        IProjectService service = new ProjectService(this._context);
        EditProjectViewModel? testModel = await service.GetEditProjectViewModelAsync("2");

        Assert.That(testModel, Is.Not.EqualTo(null));
        Assert.That(testModel.Name, Is.EqualTo("Test project 2"));
    }

    [Test]
    [Order(8)]
    public async Task Test_GetEditProjectViewModelAsync_Fail()
    {
        IProjectService service = new ProjectService(this._context);

        EditProjectViewModel? testModel = await service.GetEditProjectViewModelAsync("5");

        Assert.That(testModel, Is.EqualTo(null));
    }

    [Test]
    [Order(9)]
    public async Task Test_SaveProjectChangesAsync_Succeed()
    {
        IProjectService service = new ProjectService(this._context);
        EditProjectViewModel? testModel = await service.GetEditProjectViewModelAsync("1");
        Assert.That(testModel, Is.Not.EqualTo(null));

        testModel.Name = "Test project 1 (edited)";
        testModel.Description = "Project description (edited)";

        await service.SaveProjectChangesAsync(testModel);

        Assert.That(testModel.Name, Is.EqualTo("Test project 1 (edited)"));
        Assert.That(testModel.Description, Is.EqualTo("Project description (edited)"));
    }

    [Test]
    [Order(10)]
    public async Task Test_SaveProjectForUserAsync_Succeed()
    {
        IProjectService service = new ProjectService(this._context);
        CreateProjectViewModel testModel = new();
        ApplicationUser user = await _context.Users.FirstAsync(u => u.Id == "1");

        await service.SaveProjectForUserAsync(user.Id, testModel);

        Assert.That(user.Projects, Has.Count.EqualTo(3));
    }

    [Test]
    [Order(11)]
    public async Task Test_DeleteProjectByIdAsync_Succeed()
    {
        IProjectService service = new ProjectService(this._context);
        ApplicationUser user = await _context.Users.FirstAsync(u => u.Id == "1");

        await service.DeleteProjectByIdAsync("1");

        Assert.That(user.Projects, Has.Count.EqualTo(2));
    }

    [Test]
    [Order(11)]
    public async Task Test_DeleteProjectByIdAsync_Fail()
    {
        IProjectService service = new ProjectService(this._context);
        ApplicationUser user = await _context.Users.FirstAsync(u => u.Id == "1");

        await service.DeleteProjectByIdAsync("5"); // there is no project with id "5"

        Assert.That(user.Projects, Has.Count.EqualTo(2));
    }
}

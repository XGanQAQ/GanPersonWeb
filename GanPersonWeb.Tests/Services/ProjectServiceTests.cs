using GanPersonWeb.Data;
using GanPersonWeb.Services;
using GanPersonWeb.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GanPersonWeb.Tests.Services
{
    public class ProjectServiceTests
    {
        private DbContextOptions<GanPersonDbContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<GanPersonDbContext>()
                .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task AddProject_ShouldAddProject()
        {
            var options = CreateNewContextOptions();

            using (var context = new GanPersonDbContext(options))
            {
                var databaseService = new DatabaseService(context);
                var service = new ProjectService(databaseService);

                var project = new Project { Title = "Test Add Project", Description = "Test Description" };
                await service.AddProjectAsync(project);

                Assert.Single(await context.Projects.ToListAsync());
            }
        }

        [Fact]
        public async Task GetProjects_ShouldReturnProjects()
        {
            var options = CreateNewContextOptions();

            using (var context = new GanPersonDbContext(options))
            {
                var databaseService = new DatabaseService(context);
                var service = new ProjectService(databaseService);

                var project = new Project { Title = "Test Project", Description = "Test Description" };
                context.Projects.Add(project);
                await context.SaveChangesAsync();

                var projects = await service.GetProjectsAsync();
                Assert.Single(projects);
            }
        }

        [Fact]
        public async Task DeleteProject_ShouldRemoveProject()
        {
            var options = CreateNewContextOptions();

            using (var context = new GanPersonDbContext(options))
            {
                var databaseService = new DatabaseService(context);
                var service = new ProjectService(databaseService);

                var project = new Project { Title = "Test Project", Description = "Test Description" };
                context.Projects.Add(project);
                await context.SaveChangesAsync();

                await service.DeleteProjectAsync(project.Id);
                Assert.Empty(await context.Projects.ToListAsync());
            }
        }
    }
}

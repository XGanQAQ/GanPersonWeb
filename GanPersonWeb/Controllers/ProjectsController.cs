using GanPersonWeb.Services;
using GanPersonWeb.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GanPersonWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectService _projectService;

        public ProjectsController(ProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var projects = await _projectService.GetProjectsAsync();
            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
                return NotFound();

            return Ok(project);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddProject([FromBody] Project project)
        {
            await _projectService.AddProjectAsync(project);
            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] Project project)
        {
            if (id != project.Id)
                return BadRequest();

            await _projectService.UpdateProjectAsync(project);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            await _projectService.DeleteProjectAsync(id);
            return NoContent();
        }

        // 分页获取项目
        [HttpGet("range/{start}/{count}")]
        public async Task<IActionResult> GetProjectsInRange(int start, int count)
        {
            var projects = await _projectService.GetProjectsInRangeAsync(start, count);
            return Ok(projects);
        }

        // 获取项目总数
        [HttpGet("count")]
        public async Task<IActionResult> GetProjectsCount()
        {
            var count = await _projectService.GetProjectsCountAsync();
            return Ok(count);
        }
    }
}

using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.ProjectDTO;
using Application.Objects.DTOs.UserDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [Authorize]
        [HttpPost("create-project")]
        public async Task<ActionResult<bool>> CreateProject(CreateProjectDTO projectDTO)
        {
            try
            {
                var result = await _projectService.CreateProjectAsync(projectDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-project-by-id")]
        public async Task<ActionResult<ProjectDataDTO>> GetProjectById(string projectId)
        {
            try
            {
                var result = await _projectService.GetProjectAsync(projectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-project-list")]
        public async Task<ActionResult<List<ProjectDataDTO>>> GetProjectList()
        {
            try
            {
                var result = await _projectService.GetAllProjectsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [Authorize]
        [HttpGet("get-project-list-by-user-id")]
        public async Task<ActionResult<List<ProjectDataDTO>>> GetProjectListByUserId(string userId)
        {
            try
            {
                var result = await _projectService.GetAllProjectsByUserIdAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("delete-project")]
        public async Task<ActionResult<bool>> DeleteProject(string projectId)
        {
            try
            {
                var result = await _projectService.DeleteProjectAsync(projectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("edit-project")]
        public async Task<ActionResult<bool>> EditProject(ProjectDataDTO projectDTO)
        {
            try
            {
                var result = await _projectService.UpdateProjectAsync(projectDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            
            }
        }

        [Authorize]
        [HttpPut("edit-project-status")]
        public async Task<ActionResult<bool>> EditProjectStatus(string projectId, string status)
        {
            try
            {
                var result = await _projectService.ChangeStatusAsync(projectId, status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.TaskDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [Authorize]
        [HttpPost("create-task")]
        public async Task<ActionResult<bool>> CreateTask(CreateTaskDTO taskDTO)
        {
            try
            {
                var result = await _taskService.CreateTaskAsync(taskDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-task-by-id")]
        public async Task<ActionResult<TaskDataDTO>> GetTaskById(string taskId)
        {
            try
            {
                var result = await _taskService.GetTaskAsync(taskId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-task-list")]
        public async Task<ActionResult<List<TaskDataDTO>>> GetTaskList()
        {
            try
            {
                var result = await _taskService.GetAllTasksAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-task-list-by-project-id")]
        public async Task<ActionResult<List<TaskDataDTO>>> GetTaskListByProjectId(string projectId)
        {
            try
            {
                if (!Guid.TryParse(projectId, out var guid))
                    return BadRequest(new { message = "Invalid projectId format" });

                var result = await _taskService.GetAllTasksByProjectIdAsync(guid);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-task-list-by-user-id")]
        public async Task<ActionResult<List<TaskDataDTO>>> GetTaskListByUserId(string userId)
        {
            try
            {
                var result = await _taskService.GetAllTasksByUserIdAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("delete-task")]
        public async Task<ActionResult<bool>> DeleteTask(string taskId)
        {
            try
            {
                var result = await _taskService.DeleteTaskAsync(taskId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("edit-task")]
        public async Task<ActionResult<bool>> EditTask(TaskDataDTO taskDTO)
        {
            try
            {
                var result = await _taskService.UpdateTaskAsync(taskDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("edit-task-status")]
        public async Task<ActionResult<bool>> EditTaskStatus(string taskId, string status)
        {
            try
            {
                var result = await _taskService.ChangeStatusAsync(taskId, status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("edit-task-priority")]
        public async Task<ActionResult<bool>> EditTaskPriority(string taskId, string priority)
        {
            try
            {
                var result = await _taskService.ChangePriorityAsync(taskId, priority);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

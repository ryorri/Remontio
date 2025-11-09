using Application.Objects.DTOs.TaskDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.ServiceInterfaces
{
    public interface ITaskService
    {
        Task<bool> CreateTaskAsync(CreateTaskDTO taskDTO);
        Task<bool> UpdateTaskAsync(TaskDataDTO taskDTO);
        Task<bool> DeleteTaskAsync(string taskId);
        Task<List<TaskDataDTO>> GetAllTasksAsync();
        Task<List<TaskDataDTO>> GetAllTasksByProjectIdAsync(Guid projectId);
        Task<List<TaskDataDTO>> GetAllTasksByUserIdAsync(string userId);
        Task<TaskDataDTO> GetTaskAsync(string taskId);
        Task<bool> ChangeStatusAsync(string taskId, string status);
        Task<bool> ChangePriorityAsync(string taskId, string priority);


    }
}

using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.TaskDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class TaskService : ITaskService
    {
        public Task<bool> ChangePriorityAsync(string taskId, string priority)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ChangeStatusAsync(string taskId, string status)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateTaskAsync(CreateTaskDTO taskDTO)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteTaskAsync(string taskId)
        {
            throw new NotImplementedException();
        }

        public Task<List<TaskDataDTO>> GetAllTasksAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<TaskDataDTO>> GetAllTasksByProjectIdAsync(Guid projectId)
        {
            throw new NotImplementedException();
        }

        public Task<List<TaskDataDTO>> GetAllTasksByUserIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<TaskDataDTO> GetTaskAsync(string taskId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateTaskAsync(TaskDataDTO taskDTO)
        {
            throw new NotImplementedException();
        }
    }
}

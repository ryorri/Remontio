using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.TaskDTO;
using Application.Validators;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class TaskService : ITaskService
    {
        private readonly RemontioDbContext _dbContext;
        private readonly IMapper _mapper;

        public TaskService(RemontioDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<bool> ChangePriorityAsync(string taskId, string priority)
        {
            try
            {
                var guid = GuidValidator.ValidateGuid(taskId);

                var task = await _dbContext.Tasks.FindAsync(guid);

                if (task != null)
                {
                    task.Priority = Enum.Parse<PriorityEnum>(priority, true);
                }

                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> ChangeStatusAsync(string taskId, string status)
        {
            try
            {
                var guid = GuidValidator.ValidateGuid(taskId);

                var task = await _dbContext.Tasks.FindAsync(guid);

                if (task != null)
                {
                    task.Status = Enum.Parse<StatusEnum>(status, true);
                }

                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> CreateTaskAsync(CreateTaskDTO taskDTO)
        {
            if (taskDTO == null)
                throw new ArgumentNullException(nameof(taskDTO));

            try
            {
                var entity = _mapper.Map<Tasks>(taskDTO);
                entity.CreateAt = DateTime.UtcNow;
                await _dbContext.Tasks.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> DeleteTaskAsync(string taskId)
        {
            if (taskId == null)
                throw new ArgumentNullException(nameof(taskId));

            try
            {
                var guid = GuidValidator.ValidateGuid(taskId);

                var task = await _dbContext.Tasks.FindAsync(guid);

                if (task != null)
                    _dbContext.Tasks.Remove(task);

                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<List<TaskDataDTO>> GetAllTasksAsync()
        {
            try
            {
                var taskList = await _dbContext.Tasks
                                               .Include(x => x.User)
                                               .ToListAsync();
                return _mapper.Map<List<TaskDataDTO>>(taskList);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<List<TaskDataDTO>> GetAllTasksByProjectIdAsync(Guid projectId)
        {
            try
            {
                var taskList = await _dbContext.Tasks
                                               .Include(x => x.User)
                                               .Where(x => x.ProjectId == projectId)
                                               .ToListAsync();
                return _mapper.Map<List<TaskDataDTO>>(taskList);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<List<TaskDataDTO>> GetAllTasksByUserIdAsync(string userId)
        {
            try
            {
                var taskList = await _dbContext.Tasks
                                               .Include(x => x.User)
                                               .Where(x => x.UserId == userId)
                                               .ToListAsync();
                return _mapper.Map<List<TaskDataDTO>>(taskList);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<TaskDataDTO> GetTaskAsync(string taskId)
        {
            try
            {
                var task = await _dbContext.Tasks
                                            .Include(x => x.User)
                                            .FirstOrDefaultAsync(x => x.Id.ToString() == taskId);

                return _mapper.Map<TaskDataDTO>(task);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> UpdateTaskAsync(TaskDataDTO taskDTO)
        {
            try
            {
                var guid = GuidValidator.ValidateGuid(taskDTO.Id);

                var task = await _dbContext.Tasks.FindAsync(guid);

                if (task != null)
                {
                    task.Name = taskDTO.Name;
                    task.Description = taskDTO.Description;
                }

                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }
    }
}

using Application.Interfaces.DatabaseInterfaces;
using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.ProjectDTO;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class ProjectService : IProjectService
    {
        private readonly RemontioDbContext _dbContext;
        private readonly IMapper _mapper;
        public ProjectService(RemontioDbContext dbContext, IMapper mapper) 
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }


        public async Task<bool> CreateProjectAsync(CreateProjectDTO projectDTO)
        {
            if (projectDTO == null)
                throw new ArgumentNullException(nameof(projectDTO));

            try
            {
                var project = _mapper.Map<Project>(projectDTO);
                await _dbContext.Projects.AddAsync(project);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch(Exception ex) 
            {
                throw new ArgumentException($"Error: ${ex}");
            }
            

        }

        public async Task<bool> DeleteProjectAsync(string projectId)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));

            try
            {
                if (!Guid.TryParse(projectId, out Guid guid))
                    throw new ArgumentException("Invalid project ID format");

                var project = await _dbContext.Projects.FindAsync(guid);

                if(project != null)
                     _dbContext.Projects.Remove(project);

                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<List<ProjectDataDTO>> GetAllProjectsAsync()
        {
            try
            {
                var projectList = await _dbContext.Projects.Include(x => x.User)
                                                            .ToListAsync();
                return _mapper.Map<List<ProjectDataDTO>>(projectList);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<List<ProjectDataDTO>> GetAllProjectsByUserIdAsync(string userId)
        {
            try
            {
                var projectList = await _dbContext.Projects.Include(x => x.User)
                                                  .Where(x => x.UserId == userId)
                                                  .ToListAsync();
                return _mapper.Map<List<ProjectDataDTO>>(projectList);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<ProjectDataDTO> GetProjectAsync(string projectId)
        {
            try
            {
                var project = await _dbContext.Projects.Include(x => x.User)
                                                  .FirstOrDefaultAsync(x => x.Id.ToString() == projectId);

                return _mapper.Map<ProjectDataDTO>(project);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> UpdateProjectAsync(ProjectDataDTO projectDTO)
        {
            try
            {
                if (!Guid.TryParse(projectDTO.Id, out Guid guid))
                    throw new ArgumentException("Invalid project ID format");

                var project = await _dbContext.Projects.FindAsync(guid);

                if (project != null)
                {
                    project.Name = projectDTO.Name;
                    project.Description = projectDTO.Description;

                }
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }
        public async Task<bool> ChangeStatusAsync(string projectId, string status)
        {
            try
            {
                if (!Guid.TryParse(projectId, out Guid guid))
                    throw new ArgumentException("Invalid project ID format");

                var project = await _dbContext.Projects.FindAsync(guid);

                if (project != null)
                {
                    project.Status = Enum.Parse<StatusEnum>(status, true);

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

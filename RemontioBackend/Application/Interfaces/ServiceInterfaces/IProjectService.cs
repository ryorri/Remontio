using Application.Objects.DTOs.ProjectDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.ServiceInterfaces
{
    public interface IProjectService
    {
        Task<bool> CreateProjectAsync(CreateProjectDTO projectDTO);
        Task<bool> UpdateProjectAsync(ProjectDataDTO projectDTO);
        Task<bool> DeleteProjectAsync(string projectId);
        Task<List<ProjectDataDTO>> GetAllProjectsAsync();
        Task<List<ProjectDataDTO>> GetAllProjectsByUserIdAsync(string userId);
        Task<ProjectDataDTO> GetProjectAsync(string projectId);
        Task<bool> ChangeStatusAsync(string projectId, string status);
    }
}

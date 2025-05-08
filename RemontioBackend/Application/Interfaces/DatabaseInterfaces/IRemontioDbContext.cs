using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces.DatabaseInterfaces
{
    public interface IRemontioDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Project> Projects { get; set; }
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces.DatabaseInterfaces
{
    public interface IRemontioDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Project> Projects { get; set; }
        DbSet<Budget> Budgets { get; set; }
        DbSet<Calculations> Calculations { get; set; }
        DbSet<Photos> Photos { get; set; }
        DbSet<Room> Rooms { get; set; }
        DbSet<ShoppingList> ShoppingLists { get; set; }
        DbSet<Tasks> Tasks { get; set; }
    }
}

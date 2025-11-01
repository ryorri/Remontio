using Application.Interfaces.DatabaseInterfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class RemontioDbContext : IdentityDbContext<User>, IRemontioDbContext
    {
        public RemontioDbContext(DbContextOptions<RemontioDbContext> options) : base(options) { }
        public override DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get ; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Calculations> Calculations { get; set; }
        public DbSet<Photos> Photos { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<ShoppingList> ShoppingLists { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Wall> Walls { get; set; }
        public DbSet<Floor> Floors { get; set; }
        public DbSet<Alerts> Alerts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RemontioDbContext).Assembly);
        }
    }
}

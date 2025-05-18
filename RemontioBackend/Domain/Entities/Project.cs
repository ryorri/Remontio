using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
        public DateTime ClosedAt { get; set; }
        public StatusEnum Status { get; set; }
        public string UserId { get; set; } = string.Empty; // FK
        public User? User { get; set; }  // Nav

        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
        public ICollection<Calculations> Calculations { get; set; } = new List<Calculations>();
        public ICollection<ShoppingList> ShoppingLists { get; set; } = new List<ShoppingList>();
        public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
        public ICollection<Photos> Photos { get; set; } = new List<Photos>();
    }
}

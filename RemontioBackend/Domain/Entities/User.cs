using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class User : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty ;
        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<Calculations> Calculations { get; set; } = new List<Calculations>();
        public ICollection<ShoppingList> ShoppingLists { get; set; } = new List<ShoppingList>();
        public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
    }
}

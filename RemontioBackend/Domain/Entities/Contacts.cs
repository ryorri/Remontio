using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Contacts
    {
        public Guid Id { get; set; }
        public string Name { get; set; } =string.Empty;
        public string Description { get; set; } =string.Empty;
        public string ContactDetails {  get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool IsPrivate { get; set; }

        public string UserId { get; set; } =string.Empty ;
        public User? User { get; set; } // added navigation
    }
}

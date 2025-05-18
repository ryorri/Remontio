using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Items
{
    public class BudgetItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public float Price { get; set; }
        public float Total { get; set; }
        public float EstimatetPrice { get; set; }
    }
}

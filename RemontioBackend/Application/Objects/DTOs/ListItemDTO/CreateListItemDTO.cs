using System;

namespace Application.Objects.DTOs.ListItemDTO
{
    public class CreateListItemDTO
    {
        public required string Name { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public bool IsBought { get; set; } = false;
        public required string ShoppingListId { get; set; }
    }
}

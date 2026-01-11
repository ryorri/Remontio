using System;

namespace Application.Objects.DTOs.ListItemDTO
{
    public class ListItemDataDTO
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public bool IsBought { get; set; }
        public required string ShoppingListId { get; set; }
    }
}

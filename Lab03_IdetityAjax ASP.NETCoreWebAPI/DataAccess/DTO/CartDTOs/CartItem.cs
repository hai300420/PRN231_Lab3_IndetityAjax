using DataAccess.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.CartDTOs
{
    public class CreateOrderDTO
    {
        public int AccountId { get; set; }
        public decimal TotalAmount { get; set; }
        public string? OrderStatus { get; set; } = OrderStatusEnum.Pending.ToString();
        public List<CartItem> OrderItems { get; set; } = new List<CartItem>();
    }

    public class CartItem
    {
        public int OrchidId { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal Price { get; set; }
    }

    public class OrderItemDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

}

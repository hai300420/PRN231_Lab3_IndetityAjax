using DataAccess.DTO.OrderDetailDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.OrderDTOs
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public int AccountId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? OrderStatus { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? AccountName { get; set; }
        public string? Email { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; } = new();
    }
}

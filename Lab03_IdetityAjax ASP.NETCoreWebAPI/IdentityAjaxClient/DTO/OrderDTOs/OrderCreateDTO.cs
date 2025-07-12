using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.OrderDTOs
{
    public class OrderCreateDTO
    {
        public int AccountId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? OrderStatus { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}

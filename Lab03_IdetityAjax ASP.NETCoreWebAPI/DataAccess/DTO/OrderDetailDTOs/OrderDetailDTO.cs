using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.OrderDetailDTOs
{
    public class OrderDetailDTO
    {
        public int OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public int? OrderId { get; set; }
        public string? ProductName { get; set; } // For display
    }
}

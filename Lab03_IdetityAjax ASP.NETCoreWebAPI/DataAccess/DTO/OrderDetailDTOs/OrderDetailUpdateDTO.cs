using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.OrderDetailDTOs
{
    public class OrderDetailUpdateDTO
    {
        public int OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public int? OrderId { get; set; }
    }
}

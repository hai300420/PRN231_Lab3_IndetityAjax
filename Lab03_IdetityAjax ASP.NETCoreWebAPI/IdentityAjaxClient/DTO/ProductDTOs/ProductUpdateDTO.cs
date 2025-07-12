using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.ProductDTOs
{
    public class ProductUpdateDTO
    {
        public string ProductName { get; set; } = null!;
        public string? ProductDescription { get; set; }
        public string? ProductUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public bool? IsNatural { get; set; }
        public int CategoryId { get; set; }
    }
}

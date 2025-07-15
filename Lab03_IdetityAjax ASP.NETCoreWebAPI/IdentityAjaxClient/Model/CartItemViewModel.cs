using BusinessObjects;
using DataAccess.DTO.ProductDTOs;

namespace IdentityAjaxClient.Model
{
    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public ProductDTO Product { get; set; } = null!;
    }
}

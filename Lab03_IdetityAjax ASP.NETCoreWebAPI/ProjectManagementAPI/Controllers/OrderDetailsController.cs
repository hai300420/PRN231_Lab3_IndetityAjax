using BusinessObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interface;
using Repositories;
using DataAccess.DTO.OrderDetailDTOs;

namespace ProjectManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly IOrderDetailRepository repository = new OrderDetailRepository();

        [HttpGet]
        public ActionResult<IEnumerable<OrderDetail>> GetOrderDetails()
        {
            var ods = repository.GetOrderDetails();
            var result = ods.Select(od => new OrderDetailDTO
            {
                OrderDetailId = od.OrderDetailId,
                ProductId = od.ProductId,
                Price = od.Price,
                Quantity = od.Quantity,
                OrderId = od.OrderId,
                ProductName = od.Product?.ProductName // Product is included
            });

            return Ok(result);
        }

        [HttpPost]
        public IActionResult PostOrderDetail([FromBody] OrderDetailCreateDTO dto)
        {
            var od = new OrderDetail
            {
                ProductId = dto.ProductId,
                Price = dto.Price,
                Quantity = dto.Quantity,
                OrderId = dto.OrderId
            };

            repository.SaveOrderDetail(od);
            return Ok(new 
            { 
                Status = "success", 
                Message = "OrderDetail created successfully" 
            });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateOrderDetail(int id, [FromBody] OrderDetailUpdateDTO dto)
        {
            var existing = repository.GetOrderDetailById(id);
            if (existing == null) return NotFound();

            var od = new OrderDetail
            {
                OrderDetailId = id,
                ProductId = dto.ProductId,
                Price = dto.Price,
                Quantity = dto.Quantity,
                OrderId = dto.OrderId
            };

            repository.UpdateOrderDetail(od);
            return Ok(new 
            { 
                Status = "success", 
                Message = "OrderDetail updated successfully" 
            });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteOrderDetail(int id)
        {
            var od = repository.GetOrderDetailById(id);
            if (od == null) return NotFound();

            repository.DeleteOrderDetail(id);
            return Ok(new 
            { 
                Status = "success", 
                Message = "OrderDetail deleted successfully" 
            });

        }
    }
}

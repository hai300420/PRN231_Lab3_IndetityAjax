using BusinessObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interface;
using Repositories;
using DataAccess.DTO.OrderDTOs;
using DataAccess.DTO.OrderDetailDTOs;

namespace ProjectManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository repository = new OrderRepository();

        [HttpGet]
        public ActionResult<IEnumerable<Order>> GetOrders()
        {
            var orders = repository.GetOrders();
            var result = orders.Select(o => new OrderDTO
            {
                OrderId = o.OrderId,
                AccountId = o.AccountId,
                OrderDate = o.OrderDate,
                OrderStatus = o.OrderStatus,
                TotalAmount = o.TotalAmount,
                AccountName = o.Account?.AccountName,
                OrderDetails = o.OrderDetails.Select(od => new OrderDetailDTO
                {
                    OrderDetailId = od.OrderDetailId,
                    ProductId = od.ProductId,
                    Price = od.Price,
                    Quantity = od.Quantity,
                    ProductName = od.Product?.ProductName
                }).ToList()
            });

            return Ok(result);
        }

        [HttpPost]
        public IActionResult PostOrder([FromBody] OrderCreateDTO dto)
        {
            var order = new Order
            {
                AccountId = dto.AccountId,
                OrderDate = dto.OrderDate,
                OrderStatus = dto.OrderStatus,
                TotalAmount = dto.TotalAmount
            };

            repository.SaveOrder(order);
            return Ok(new 
            { 
                Status = "success", 
                Message = "Order created successfully." 
            });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateOrder(int id, [FromBody] OrderUpdateDTO dto)
        {
            var existing = repository.GetOrderById(id);
            if (existing == null) return NotFound();

            existing.AccountId = dto.AccountId;
            existing.OrderDate = dto.OrderDate;
            existing.OrderStatus = dto.OrderStatus;
            existing.TotalAmount = dto.TotalAmount;

            repository.UpdateOrder(existing);
            return Ok(new 
            { 
                Status = "success", 
                Message = "Order updated successfully." 
            });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id)
        {
            var o = repository.GetOrderById(id);
            if (o == null) return NotFound();

            repository.DeleteOrder(id);
            return Ok(new 
            { 
                Status = "success",
                Message = "Order deleted successfully." 
            });
        }
    }
}

using BusinessObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interface;
using Repositories;
using DataAccess.DTO.OrderDTOs;
using DataAccess.DTO.OrderDetailDTOs;
using DataAccess.DTO;
using DataAccess.Enum;

namespace ProjectManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository repository = new OrderRepository();

        //[HttpGet]
        //public ActionResult<IEnumerable<Order>> GetOrders()
        //{
        //    var orders = repository.GetOrders();
        //    var result = orders.Select(o => new OrderDTO
        //    {
        //        OrderId = o.OrderId,
        //        AccountId = o.AccountId,
        //        OrderDate = o.OrderDate,
        //        OrderStatus = o.OrderStatus,
        //        TotalAmount = o.TotalAmount,
        //        AccountName = o.Account?.AccountName,
        //        OrderDetails = o.OrderDetails.Select(od => new OrderDetailDTO
        //        {
        //            OrderDetailId = od.OrderDetailId,
        //            ProductId = od.ProductId,
        //            Price = od.Price,
        //            Quantity = od.Quantity,
        //            ProductName = od.Product?.ProductName
        //        }).ToList()
        //    });

        //    return Ok(result);
        //}

        [HttpGet]
        public ActionResult<PagedResult<Order>> GetOrders(
                    int? id = null,
                    OrderStatusEnum? orderStatus = null,
                    string? customer = null,
                    DateTime? startDate = null,
                    DateTime? endDate = null,
                    int page = 1,
                    int pageSize = 10)
        {
            var orders = repository.GetOrders();

            if (id.HasValue)
            {
                orders = orders.Where(p => p.OrderId == id.Value).ToList();
            }

            // Filtering
            if (orderStatus.HasValue)
            {
                var statusString = orderStatus.Value.ToString();
                orders = orders.Where(o => o.OrderStatus == statusString).ToList();
            }


            if (!string.IsNullOrWhiteSpace(customer))
            {
                orders = orders.Where(o =>
                    o.Account != null &&
                    (o.Account.Email.Contains(customer, StringComparison.OrdinalIgnoreCase) ||
                     o.Account.AccountName.Contains(customer, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            if (startDate.HasValue)
            {
                orders = orders.Where(o => o.OrderDate >= startDate.Value).ToList();
            }

            if (endDate.HasValue)
            {
                orders = orders.Where(o => o.OrderDate <= endDate.Value).ToList();
            }

            var totalCount = orders.Count();

            var items = orders
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderDTO
                {
                    OrderId = o.OrderId,
                    AccountId = o.AccountId,
                    OrderDate = o.OrderDate,
                    OrderStatus = o.OrderStatus,
                    TotalAmount = o.TotalAmount,
                    AccountName = o.Account?.AccountName,
                    Email = o.Account?.Email,
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailDTO
                    {
                        OrderDetailId = od.OrderDetailId,
                        ProductId = od.ProductId,
                        Price = od.Price,
                        Quantity = od.Quantity,
                        ProductName = od.Product?.ProductName,
                        ProductUrl = od.Product?.ProductUrl,
                        CategoryName = od.Product?.Category?.CategoryName
                    }).ToList()
                })
                .ToList();

            return Ok(new PagedResultDetail<OrderDTO>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            });
        }

        [HttpGet("{id}")]
        public ActionResult<Order> GetOrder(int id)
        {
            var orders = repository.GetOrderById(id);
            if (orders == null) return NotFound();

            var dto = new OrderDTO
            {
                OrderId = orders.OrderId,
                AccountId = orders.AccountId,
                OrderDate = orders.OrderDate,
                OrderStatus = orders.OrderStatus,
                TotalAmount = orders.TotalAmount,
                AccountName = orders.Account?.AccountName,
                Email = orders.Account?.Email,
                OrderDetails = orders.OrderDetails.Select(od => new OrderDetailDTO
                {
                    OrderDetailId = od.OrderDetailId,
                    ProductId = od.ProductId,
                    Price = od.Price,
                    Quantity = od.Quantity,
                    ProductName = od.Product?.ProductName,
                    ProductUrl = od.Product?.ProductUrl,
                    CategoryName = od.Product?.Category?.CategoryName
                }).ToList()
            };

            return Ok(dto);
        }

        //[HttpGet("{id}")]
        //public ActionResult<OrderDTO> GetOrder(int id)
        //{
        //    var orders = repository.GetOrderById(id);
        //    if (orders == null) return NotFound();

        //    var dto = new OrderDTO
        //    {
        //        OrderId = orders.OrderId,
        //        AccountId = orders.AccountId,
        //        OrderDate = orders.OrderDate,
        //        OrderStatus = orders.OrderStatus,
        //        TotalAmount = orders.TotalAmount,
        //        AccountName = orders.Account?.AccountName,
        //        Email = orders.Account?.Email,
        //        OrderDetails = orders.OrderDetails.Select(od => new OrderDetailDTO
        //        {
        //            OrderDetailId = od.OrderDetailId,
        //            ProductId = od.ProductId,
        //            Price = od.Price,
        //            Quantity = od.Quantity,
        //            ProductName = od.Product?.ProductName
        //        }).ToList()
        //    };

        //    return Ok(dto);
        //}


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

            if (string.IsNullOrEmpty(order.OrderStatus))
            {
                order.OrderStatus = OrderStatusEnum.Pending.ToString();
            }

            repository.SaveOrder(order);
            //return Ok(new 
            //{ 
            //    Status = "success", 
            //    Message = "Order created successfully." 
            //});
            return Ok(order); 

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

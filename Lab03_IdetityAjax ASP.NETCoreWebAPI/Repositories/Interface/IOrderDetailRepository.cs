using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IOrderDetailRepository
    {
        List<OrderDetail> GetOrderDetails();
        List<OrderDetail> GetOrderDetailsByOrderId(int orderId);
        OrderDetail GetOrderDetailById(int id);
        void SaveOrderDetail(OrderDetail detail);
        void UpdateOrderDetail(OrderDetail detail);
        void DeleteOrderDetail(int id);
    }
}

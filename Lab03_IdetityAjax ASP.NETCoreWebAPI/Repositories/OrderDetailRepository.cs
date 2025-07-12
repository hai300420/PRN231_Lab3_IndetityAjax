using BusinessObjects;
using DataAccess.DAO;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        public List<OrderDetail> GetOrderDetails() => OrderDetailDAO.GetOrderDetails();

        public List<OrderDetail> GetOrderDetailsByOrderId(int orderId) =>
            OrderDetailDAO.GetOrderDetailsByOrderId(orderId);

        public OrderDetail GetOrderDetailById(int id) => OrderDetailDAO.GetOrderDetailById(id);

        public void SaveOrderDetail(OrderDetail detail) => OrderDetailDAO.SaveOrderDetail(detail);

        public void UpdateOrderDetail(OrderDetail detail) => OrderDetailDAO.UpdateOrderDetail(detail);

        public void DeleteOrderDetail(int id) => OrderDetailDAO.DeleteOrderDetail(id);
    }
}

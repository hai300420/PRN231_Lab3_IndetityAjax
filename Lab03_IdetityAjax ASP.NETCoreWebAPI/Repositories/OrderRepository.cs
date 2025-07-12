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
    public class OrderRepository : IOrderRepository
    {
        public List<Order> GetOrders() => OrderDAO.GetOrders();

        public Order GetOrderById(int id) => OrderDAO.GetOrderById(id);

        public void SaveOrder(Order order) => OrderDAO.SaveOrder(order);

        public void UpdateOrder(Order order) => OrderDAO.UpdateOrder(order);

        public void DeleteOrder(int id) => OrderDAO.DeleteOrder(id);
    }
}

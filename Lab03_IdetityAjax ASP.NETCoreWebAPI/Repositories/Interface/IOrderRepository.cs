using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IOrderRepository
    {
        List<Order> GetOrders();
        Order GetOrderById(int id);
        void SaveOrder(Order order);
        void UpdateOrder(Order order);
        void DeleteOrder(int id);
    }
}

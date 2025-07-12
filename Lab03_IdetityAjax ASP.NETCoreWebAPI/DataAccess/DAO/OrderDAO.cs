using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class OrderDAO
    {
        public static List<Order> GetOrders()
        {
            try
            {
                using var context = new MyStoreDbContext();
                return context.Orders.Include(acc => acc.Account)
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static Order? GetOrderById(int id)
        {
            try
            {
                using var context = new MyStoreDbContext();
                return context.Orders.Include(acc => acc.Account)
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                    .FirstOrDefault(o => o.OrderId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void SaveOrder(Order order)
        {
            try
            {
                using var context = new MyStoreDbContext();
                context.Orders.Add(order);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void UpdateOrder(Order order)
        {
            try
            {
                using var context = new MyStoreDbContext();
                context.Entry(order).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void DeleteOrder(int id)
        {
            try
            {
                using var context = new MyStoreDbContext();
                var o = context.Orders.FirstOrDefault(x => x.OrderId == id);
                if (o != null)
                {
                    context.Orders.Remove(o);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

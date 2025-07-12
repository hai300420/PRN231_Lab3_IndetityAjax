using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class OrderDetailDAO
    {
        public static List<OrderDetail> GetOrderDetails()
        {
            try
            {
                using var context = new MyStoreDbContext();
                return context.OrderDetails.Include(p => p.Product).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static List<OrderDetail> GetOrderDetailsByOrderId(int orderId)
        {
            try
            {
                using var context = new MyStoreDbContext();
                return context.OrderDetails.Include(p => p.Product).Where(d => d.OrderId == orderId).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static OrderDetail? GetOrderDetailById(int id)
        {
            try
            {
                using var context = new MyStoreDbContext();
                return context.OrderDetails.Include(p => p.Product).FirstOrDefault(x => x.OrderDetailId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void SaveOrderDetail(OrderDetail detail)
        {
            try
            {
                using var context = new MyStoreDbContext();
                context.OrderDetails.Add(detail);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void UpdateOrderDetail(OrderDetail detail)
        {
            try
            {
                using var context = new MyStoreDbContext();
                context.Entry(detail).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void DeleteOrderDetail(int id)
        {
            try
            {
                using var context = new MyStoreDbContext();
                var d = context.OrderDetails.FirstOrDefault(x => x.OrderDetailId == id);
                if (d != null)
                {
                    context.OrderDetails.Remove(d);
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

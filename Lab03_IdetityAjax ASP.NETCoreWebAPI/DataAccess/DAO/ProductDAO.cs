using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class ProductDAO
    {
        //public static List<Product> GetProducts()
        //{
        //    var listProducts = new List<Product>();
        //    try
        //    {
        //        using (var context = new MyStoreDbContext())
        //        {
        //            listProducts = context.Products.Include(p => p.Category).ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //    return listProducts;
        //}

        public static IQueryable<Product> GetProducts(MyStoreDbContext context)
        {
            return context.Products.Include(p => p.Category).AsQueryable();
        }


        public static Product FindProductById(int proId)
        {
            Product p = new Product();
            try
            {
                using (var context = new MyStoreDbContext())
                {
                    p = context.Products.Include(p => p.Category).SingleOrDefault(x => x.ProductId == proId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return p;
        }

        public static void SaveProduct(Product p)
        {
            try
            {
                using (var context = new MyStoreDbContext())
                {
                    context.Products.Add(p);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void UpdateProduct(Product p)
        {
            try
            {
                using (var context = new MyStoreDbContext())
                {
                    context.Entry(p).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void DeleteProduct(Product p)
        {
            try
            {
                using (var context = new MyStoreDbContext())
                {
                    var p1 = context.Products.SingleOrDefault(x => x.ProductId == p.ProductId);

                    context.Products.Remove(p1);

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

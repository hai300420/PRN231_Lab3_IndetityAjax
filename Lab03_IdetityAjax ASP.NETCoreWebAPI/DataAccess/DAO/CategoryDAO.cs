using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class CategoryDAO
    {
        public static List<Category> GetCategories()
        {
            var listCategories = new List<Category>();
            try
            {
                using (var context = new MyStoreDbContext())
                {
                    listCategories = context.Categories.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listCategories;
        }

        public static Category? GetCategoryById(int id)
        {
            try
            {
                using var context = new MyStoreDbContext();
                return context.Categories.FirstOrDefault(c => c.CategoryId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void SaveCategory(Category category)
        {
            try
            {
                using var context = new MyStoreDbContext();
                context.Categories.Add(category);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void UpdateCategory(Category category)
        {
            try
            {
                using var context = new MyStoreDbContext();
                context.Entry(category).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void DeleteCategory(int id)
        {
            try
            {
                using var context = new MyStoreDbContext();
                var c = context.Categories.FirstOrDefault(c => c.CategoryId == id);
                if (c != null)
                {
                    context.Categories.Remove(c);
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

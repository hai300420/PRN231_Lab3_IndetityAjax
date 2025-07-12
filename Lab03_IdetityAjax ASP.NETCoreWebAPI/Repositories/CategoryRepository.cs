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
    public class CategoryRepository : ICategoryRepository
    {
        public List<Category> GetCategories() => CategoryDAO.GetCategories();

        public Category GetCategoryById(int id) => CategoryDAO.GetCategoryById(id);

        public void SaveCategory(Category category) => CategoryDAO.SaveCategory(category);

        public void UpdateCategory(Category category) => CategoryDAO.UpdateCategory(category);

        public void DeleteCategory(int id) => CategoryDAO.DeleteCategory(id);
    }
}

using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface ICategoryRepository
    {
        List<Category> GetCategories();
        Category GetCategoryById(int id);
        void SaveCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(int id);
    }
}

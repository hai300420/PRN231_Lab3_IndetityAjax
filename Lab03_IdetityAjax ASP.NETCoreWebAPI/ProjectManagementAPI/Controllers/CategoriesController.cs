using BusinessObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interface;
using Repositories;
using DataAccess.DTO.CategoryDTOs;

namespace ProjectManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository repository = new CategoryRepository();

        [HttpGet]
        public ActionResult<IEnumerable<CategoryDTO>> GetCategories()
        {
            var categories = repository.GetCategories();
            var result = categories.Select(c => new CategoryDTO
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName
            });

            return Ok(result);
        }

        [HttpPost]
        public IActionResult PostCategory([FromBody] CategoryCreateDTO dto)
        {
            var category = new Category
            {
                CategoryName = dto.CategoryName
            };

            repository.SaveCategory(category);
            return Ok(new 
            { 
                Status = "Success", 
                Message = "Category created successfully." 
            });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCategory(int id, [FromBody] CategoryUpdateDTO dto)
        {
            var existing = repository.GetCategoryById(id);
            if (existing == null) return NotFound();

            existing.CategoryName = dto.CategoryName;

            repository.UpdateCategory(existing);
            return Ok(new 
            { 
                Status = "Success", 
                Message = "Category updated successfully." 
            });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            var existing = repository.GetCategoryById(id);
            if (existing == null) return NotFound();

            repository.DeleteCategory(id);
            return Ok(new 
            { 
                Status = "Success", 
                Message = "Category deleted successfully." 
            });

        }
    }
}

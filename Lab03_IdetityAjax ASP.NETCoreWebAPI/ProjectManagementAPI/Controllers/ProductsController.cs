using BusinessObjects;
using DataAccess.DTO.ProductDTOs;
using DataAccess.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Repositories.Interface;

namespace ProjectManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private IProductRepository repository = new ProductRepository();
        //[HttpGet]
        //public ActionResult<IEnumerable<Product>> GetProducts(int page = 1, int pageSize = 6)
        //{
        //    var products = repository.GetProducts();

        //    var totalCount = products.Count();
        //    var items = products
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .Select(p => new ProductDTO
        //        {
        //            ProductId = p.ProductId,
        //            ProductName = p.ProductName,
        //            ProductDescription = p.ProductDescription,
        //            ProductUrl = p.ProductUrl,
        //            UnitPrice = p.UnitPrice,
        //            IsNatural = p.IsNatural,
        //            CategoryId = p.CategoryId,
        //            CategoryName = p.Category?.CategoryName
        //        }).ToList();

        //    return Ok(new PagedResult<ProductDTO>
        //    {
        //        Items = items,
        //        TotalCount = totalCount
        //    });
        //}

        [HttpGet]
        public ActionResult<PagedResult<ProductDTO>> GetProducts(
             int? id = null,
             string? nameSearch = null,
             int? categoryId = null,
             bool? isNatural = null,
             int page = 1,
             int pageSize = 6)
        {
            var products = repository.GetProducts();

            if (id.HasValue)
            {
                products = products.Where(p => p.ProductId == id.Value).ToList();
            }
            // Apply filters
            if (!string.IsNullOrWhiteSpace(nameSearch))
            {
                products = products.Where(p => p.ProductName.Contains(nameSearch, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value).ToList();
            }

            if (isNatural.HasValue)
            {
                products = products.Where(p => p.IsNatural == isNatural.Value).ToList();
            }

            var totalCount = products.Count();

            var items = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductDTO
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductDescription = p.ProductDescription,
                    ProductUrl = p.ProductUrl,
                    UnitPrice = p.UnitPrice,
                    IsNatural = p.IsNatural,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category?.CategoryName
                }).ToList();

            return Ok(new PagedResult<ProductDTO>
            {
                Items = items,
                TotalCount = totalCount
            });
        }


        [HttpGet("{id}")]
        public ActionResult<ProductDTO> GetProductById(int id)
        {
            var product = repository.GetProductById(id);

            if (product == null)
            {
                return NotFound(new { Message = $"Product with ID {id} not found." });
            }

            var productDto = new ProductDTO
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                ProductUrl = product.ProductUrl,
                UnitPrice = product.UnitPrice,
                IsNatural = product.IsNatural,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.CategoryName
            };

            return Ok(productDto);
        }


        [HttpPost]
        public IActionResult PostProduct([FromBody] ProductCreateDTO dto)
        {
            var product = new Product
            {
                ProductName = dto.ProductName,
                ProductDescription = dto.ProductDescription,
                ProductUrl = dto.ProductUrl,
                CategoryId = dto.CategoryId,
                UnitPrice = dto.UnitPrice,
                IsNatural = dto.IsNatural
            };

            repository.SaveProduct(product);
            return Ok(new 
            { 
                Status = "Success", 
                Message = "Product created successfully." 
            });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] ProductUpdateDTO dto)
        {
            var existing = repository.GetProductById(id);
            if (existing == null) return NotFound();

            existing.ProductName = dto.ProductName;
            existing.ProductDescription = dto.ProductDescription;
            existing.ProductUrl = dto.ProductUrl;
            existing.CategoryId = dto.CategoryId;
            existing.UnitPrice = dto.UnitPrice;
            existing.IsNatural = dto.IsNatural;

            repository.UpdateProduct(existing);
            return Ok(new 
            { 
                Status = "Success", 
                Message = "Product updated successfully." 
            });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = repository.GetProductById(id);
            if (product == null) return NotFound();

            repository.DeleteProduct(product);
            return Ok(new 
            { 
                Status = "Success", 
                Message = "Product deleted successfully." 
            });
        }



    }
}

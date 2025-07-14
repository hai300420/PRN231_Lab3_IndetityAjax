using DataAccess.DTO.CategoryDTOs;
using DataAccess.DTO.ProductDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.Text;

namespace IdentityAjaxClient.Pages.ProductPage.Management
{
    public class EditModel : PageModel
    {
        private readonly ILogger<EditModel> _logger;
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _environment;

        public EditModel(ILogger<EditModel> logger, IHttpClientFactory factory, IWebHostEnvironment environment)
        {
            _logger = logger;
            _httpClient = factory.CreateClient("API");
            _environment = environment;
        }

        [BindProperty]
        public ProductUpdateDTO Product { get; set; } = default!;
        public SelectList? CategoryList { get; set; }
        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                // Fetch product data
                var productResponse = await _httpClient.GetAsync($"Products/{id}");
                if (!productResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    // return RedirectToPage("./Index");
                    return Page();
                }

                var productDto = await productResponse.Content.ReadFromJsonAsync<ProductUpdateDTO>();
                if (productDto == null)
                {
                    TempData["ErrorMessage"] = "Unable to load product.";
                    // return RedirectToPage("./Index");
                    return Page();
                }

                Product = productDto;

                // Fetch categories
                var categoryResponse = await _httpClient.GetAsync("Categories");
                if (categoryResponse.IsSuccessStatusCode)
                {
                    var categories = await categoryResponse.Content.ReadFromJsonAsync<List<CategoryDTO>>();
                    CategoryList = new SelectList(categories, "CategoryId", "CategoryName");
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading product.");
                TempData["ErrorMessage"] = "An error occurred.";
                return RedirectToPage("./Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors and try again.";
                return Page();
            }

            try
            {
                // Handle image upload if provided
                if (ImageFile != null)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    // Assuming you host the image in wwwroot/uploads
                    Product.ProductUrl = $"/uploads/{uniqueFileName}";
                }

                // Send update request
                var content = new StringContent(JsonSerializer.Serialize(Product), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"Products/{id}", content);

                // Call API to get category
                var categories = await _httpClient.GetAsync($"Categories");
                if (categories.IsSuccessStatusCode)
                {
                    var categoriesList = await categories.Content.ReadFromJsonAsync<List<CategoryDTO>>();
                    CategoryList = new SelectList(categoriesList, "CategoryId", "CategoryName");
                }

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Product updated successfully.";
                    return Page();
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update product.";
                    return Page();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product.");
                TempData["ErrorMessage"] = "An unexpected error occurred.";
                return Page();
            }
        }
    }
}

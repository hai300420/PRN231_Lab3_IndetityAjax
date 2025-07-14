using DataAccess.DTO.CategoryDTOs;
using DataAccess.DTO.ProductDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.Text;

namespace IdentityAjaxClient.Pages.ProductPage.Management
{
    public class CreateModel : PageModel
    {
        private readonly ILogger<EditModel> _logger;
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _environment;

        public CreateModel(ILogger<EditModel> logger, IHttpClientFactory factory, IWebHostEnvironment environment)
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

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("Categories");
                if (response.IsSuccessStatusCode)
                {
                    var categories = await response.Content.ReadFromJsonAsync<List<CategoryDTO>>();
                    CategoryList = new SelectList(categories, "CategoryId", "CategoryName");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to load categories.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading categories.");
                TempData["ErrorMessage"] = "An error occurred while loading the form.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(); // ensure dropdown stays populated
                TempData["ErrorMessage"] = "Please fix the validation errors.";
                return Page();
            }

            try
            {
                // Handle image upload
                if (ImageFile != null)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    Product.ProductUrl = $"/uploads/{uniqueFileName}";
                }

                var json = JsonSerializer.Serialize(Product);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("Products", content);

                await LoadCategoriesAsync(); // ensure dropdown stays populated after post

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Product created successfully!";
                    ModelState.Clear();
                    Product = new(); // clear the form
                    return Page(); // stay on page
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to create product.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product.");
                TempData["ErrorMessage"] = "An unexpected error occurred.";
                await LoadCategoriesAsync();
                return Page();
            }
        }

        private async Task LoadCategoriesAsync()
        {
            var response = await _httpClient.GetAsync("Categories");
            if (response.IsSuccessStatusCode)
            {
                var categories = await response.Content.ReadFromJsonAsync<List<CategoryDTO>>();
                CategoryList = new SelectList(categories, "CategoryId", "CategoryName");
            }
        }
    }
}

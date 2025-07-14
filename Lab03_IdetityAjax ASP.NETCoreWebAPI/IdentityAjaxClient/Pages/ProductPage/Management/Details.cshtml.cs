using DataAccess.DTO.ProductDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityAjaxClient.Pages.ProductPage.Management
{
    public class DetailsModel : PageModel
    {
        private readonly ILogger<DetailsModel> _logger;
        private readonly HttpClient _httpClient;

        public DetailsModel(ILogger<DetailsModel> logger, IHttpClientFactory factory)
        {
            _logger = logger;
            _httpClient = factory.CreateClient("API");
        }

        public ProductDTO Product { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Products/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToPage("./Index");
                }

                var product = await response.Content.ReadFromJsonAsync<ProductDTO>();
                if (product == null)
                {
                    TempData["ErrorMessage"] = "Failed to load product.";
                    return RedirectToPage("./Index");
                }

                Product = product;
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product details.");
                TempData["ErrorMessage"] = "An unexpected error occurred.";
                return RedirectToPage("./Index");
            }
        }
    }
}

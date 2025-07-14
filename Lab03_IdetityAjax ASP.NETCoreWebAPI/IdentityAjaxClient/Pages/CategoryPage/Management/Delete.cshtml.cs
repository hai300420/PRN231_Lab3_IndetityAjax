using BusinessObjects;
using DataAccess.DTO.CategoryDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityAjaxClient.Pages.CategoryPage.Management
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(IHttpClientFactory httpClientFactory, ILogger<DeleteModel> logger)
        {
            _httpClient = httpClientFactory.CreateClient("API");
            _logger = logger;
        }

        [BindProperty]
        public Category Category { get; set; } = default!;
        public bool HasRelatedOrchids { get; set; }
        public int RelatedOrchidsCount { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Categories/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    StatusMessage = "Error: Category not found.";
                    return RedirectToPage("./Index");
                }

                var category = await response.Content.ReadFromJsonAsync<Category>();
                if (category == null)
                {
                    StatusMessage = "Error: Failed to load category.";
                    return RedirectToPage("./Index");
                }

                Category = category;

                // You can fetch orchid count by category ID here
                // Replace this part with your actual logic if available in your API
                var countResponse = await _httpClient.GetAsync($"Categories/{id}/orchid-count");
                if (countResponse.IsSuccessStatusCode)
                {
                    var count = await countResponse.Content.ReadFromJsonAsync<int>();
                    RelatedOrchidsCount = count;
                    HasRelatedOrchids = count > 0;
                }
                else
                {
                    RelatedOrchidsCount = 0;
                    HasRelatedOrchids = false;
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading category for deletion.");
                StatusMessage = "Error: An unexpected error occurred.";
                return RedirectToPage("./Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Category == null || Category.CategoryId <= 0)
            {
                StatusMessage = "Error: Invalid category.";
                return RedirectToPage("./Index");
            }

            try
            {
                var response = await _httpClient.DeleteAsync($"Categories/{Category.CategoryId}");

                if (response.IsSuccessStatusCode)
                {
                    StatusMessage = "Category deleted successfully!";
                    return RedirectToPage("./Index");
                }
                else
                {
                    StatusMessage = "Error: Failed to delete category.";
                    return RedirectToPage("./Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category.");
                StatusMessage = "Error: An unexpected error occurred.";
                return RedirectToPage("./Index");
            }
        }

    }
}

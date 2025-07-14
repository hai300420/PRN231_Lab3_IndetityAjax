using BusinessObjects;
using DataAccess.DTO.CategoryDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityAjaxClient.Pages.CategoryPage.Management
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly HttpClient _httpClient;

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public List<CategoryDTO> Categories { get; set; } = new();

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("Categories");

                if (!response.IsSuccessStatusCode)
                {
                    StatusMessage = "Error: Failed to load categories.";
                    Categories = new List<CategoryDTO>();
                    return Page();
                }

                var categories = await response.Content.ReadFromJsonAsync<List<CategoryDTO>>();
                Categories = categories ?? new List<CategoryDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading categories.");
                StatusMessage = "Error: An unexpected error occurred.";
            }

            return Page();
        }
    }
}

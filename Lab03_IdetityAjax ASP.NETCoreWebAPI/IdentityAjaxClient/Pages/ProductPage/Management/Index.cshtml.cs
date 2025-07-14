using DataAccess.DTO;
using DataAccess.DTO.CategoryDTOs;
using DataAccess.DTO.ProductDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace IdentityAjaxClient.Pages.ProductPage.Management
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
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int TotalPages { get; set; }

        public List<ProductDTO> Products { get; set; } = new();
        public SelectList? CategoryList { get; set; }

        
        public string? ErrorMessage { get; set; }

        // Search property
        [BindProperty(SupportsGet = true)]
        public string? NameSearch { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CategoryIdSearch { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool? IsNatural { get; set; }

        public async Task OnGetAsync()
        {
            try
            {

                // Call API to get paginated products
                // var products = await _httpClient.GetAsync($"Products?page={PageNumber}&pageSize=6");
                var query = $"Products?page={PageNumber}&pageSize=6";

                if (!string.IsNullOrWhiteSpace(NameSearch))
                    query += $"&name={Uri.EscapeDataString(NameSearch)}";

                if (CategoryIdSearch.HasValue)
                    query += $"&categoryId={CategoryIdSearch.Value}";

                if (IsNatural.HasValue)
                    query += $"&isNatural={IsNatural.Value.ToString().ToLower()}"; // true/false as lowercase for consistency

                var products = await _httpClient.GetAsync(query);

                // Call API to get category
                var categories = await _httpClient.GetAsync($"Categories");

                if (products.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var response = await products.Content.ReadFromJsonAsync<PagedResult<ProductDTO>>(options);
                    if (response != null)
                    {
                        Products = response.Items ?? new();
                        TotalPages = (int)Math.Ceiling((double)response.TotalCount / 6);
                    }
                    else
                    {
                        ErrorMessage = "API returned an empty result.";
                    }
                }
                else
                {
                    ErrorMessage = "API returned non-success status: " + products.StatusCode;
                    Products = new List<ProductDTO>();
                }

                if (categories.IsSuccessStatusCode)
                {
                    var categoriesList = await categories.Content.ReadFromJsonAsync<List<CategoryDTO>>();
                    CategoryList = new SelectList(categoriesList, "CategoryId", "CategoryName");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching products.");
                ErrorMessage = "An unexpected error occurred.";
                Products = new List<ProductDTO>();
            }
        }
    }
}

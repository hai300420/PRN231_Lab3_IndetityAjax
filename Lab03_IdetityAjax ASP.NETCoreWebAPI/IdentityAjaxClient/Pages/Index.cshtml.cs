using DataAccess.DTO;
using DataAccess.DTO.ProductDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Text.Json;

namespace IdentityAjaxClient.Pages
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

        [BindProperty(SupportsGet = true, Name = "pageNumber")]
        public int Page { get; set; } = 1;

        public int TotalPages { get; set; }

        public List<ProductDTO> Products { get; set; } = new();

        public bool IsCustomer { get; private set; }

        public string? ErrorMessage { get; set; }

        // Show token to check
        public List<Claim> UserClaims { get; set; } = new();


        public async Task OnGetAsync()
        {
            // Info in token
            var token = HttpContext.Session.GetString("JWToken");

            if (!string.IsNullOrEmpty(token))
            {
                var claims = JwtHelper.DecodeJwt(token);
                UserClaims = claims.ToList();
            }
            // end token
            if (Page <= 0)
                Page = 1;
            try
            {
                // Check if current user is customer
                var userRole = HttpContext.Session.GetString("UserRole");
                IsCustomer = !string.IsNullOrEmpty(userRole) && userRole.Equals("Customer", StringComparison.OrdinalIgnoreCase);

                // Call API to get paginated products
                var httpResponse = await _httpClient.GetAsync($"Products?page={Page}&pageSize=6");

                if (httpResponse.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var response = await httpResponse.Content.ReadFromJsonAsync<PagedResult<ProductDTO>>(options);
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
                    ErrorMessage = "API returned non-success status: " + httpResponse.StatusCode;
                    Products = new List<ProductDTO>();
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

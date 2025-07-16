using DataAccess.DTO;
using DataAccess.DTO.CartDTOs;
using DataAccess.DTO.CategoryDTOs;
using DataAccess.DTO.ProductDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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


        public SelectList? CategoryList { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }

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

            // load category list
            var categories = await _httpClient.GetAsync("Categories");
            if (categories.IsSuccessStatusCode)
            {
                var categoriesList = await categories.Content.ReadFromJsonAsync<List<CategoryDTO>>();
                CategoryList = new SelectList(categoriesList, "CategoryId", "CategoryName");
            }

            try
            {
                // Check if current user is customer
                var userRole = HttpContext.Session.GetString("UserRole");
                IsCustomer = !string.IsNullOrEmpty(userRole) && userRole.Equals("Customer", StringComparison.OrdinalIgnoreCase);

                // Call API to get paginated products
                // var httpResponse = await _httpClient.GetAsync($"Products?page={Page}&pageSize=6");
                // Get query params from form (via model binding)
                var name = Request.Query["name"].ToString();
                var categoryId = Request.Query["categoryId"].ToString();
                var isNatural = Request.Query["isNatural"].ToString();

                // Build query string dynamically
                var queryParams = new List<string>
                {
                    $"page={Page}",
                    "pageSize=6"
                };

                if (!string.IsNullOrEmpty(name))
                    queryParams.Add($"nameSearch={Uri.EscapeDataString(name)}");

                if (!string.IsNullOrEmpty(categoryId))
                    queryParams.Add($"categoryId={Uri.EscapeDataString(categoryId)}");

                if (!string.IsNullOrEmpty(isNatural))
                    queryParams.Add($"isNatural={Uri.EscapeDataString(isNatural)}");

                var queryString = string.Join("&", queryParams);

                // Make API request with all filters
                var httpResponse = await _httpClient.GetAsync($"Products?{queryString}");

                

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



        public async Task<IActionResult> OnPostAddToCartAsync(int orchidId, int quantity = 1)
        {
            if (orchidId <= 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Invalid orchid ID."
                });
            }

            // Ensure quantity is at least 1
            quantity = Math.Max(1, quantity);

            try
            {
                // Get user ID from session
                var userId = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToPage("/AuthPage/Login");
                }

                // Fetch the orchid from API to get its details
                // var orchidResponse = await _httpClient.GetAsync($"products?id={orchidId}");
                var orchidResponse = await _httpClient.GetAsync($"products/{orchidId}");

                if (!orchidResponse.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                // var orchidPaginatedList = await orchidResponse.Content.ReadFromJsonAsync<PagedResultDetail<Product>>();
                // Product? orchid = orchidPaginatedList?.Items.FirstOrDefault();
                var orchid = await orchidResponse.Content.ReadFromJsonAsync<ProductDTO>();




                if (orchid == null)
                {
                    TempData["ErrorMessage"] = "Could not find the specified orchid.";
                    return RedirectToPage();
                }

                // Get current cart
                var cartJson = HttpContext.Session.GetString("Cart");
                var cart = string.IsNullOrEmpty(cartJson)
                    ? new List<CartItem>()
                    : JsonSerializer.Deserialize<List<CartItem>>(cartJson);

                if (cart == null)
                {
                    cart = new List<CartItem>();
                }

                // Add item to cart
                var existingItem = cart.FirstOrDefault(x => x.OrchidId == orchidId);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    // cart.Add(new CartItem { OrchidId = orchidId, Quantity = quantity });
                    cart.Add(new CartItem
                    {
                        OrchidId = orchidId,
                        Quantity = quantity,
                        Price = orchid.UnitPrice
                    });

                }

                // Save cart
                HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));

                // Make sure the response type is set to JSON
                Response.ContentType = "application/json";

                return new JsonResult(new
                {
                    success = true,
                    message = existingItem != null
                        ? $"{orchid.ProductName} quantity increased by {quantity} in your cart!"
                        : $"{orchid.ProductName} added to your cart with quantity {quantity}!",
                    cartCount = cart.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart");
                TempData["ErrorMessage"] = $"Error adding item to cart: {ex.Message}";
                return RedirectToPage();
            }
        }

    }
}

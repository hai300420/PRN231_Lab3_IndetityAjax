using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataAccess.DTO.ProductDTOs;
using System.Text.Json;
using Azure;
using BusinessObjects;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.CompilerServices;
using System;
using IdentityAjaxClient.Model;
using DataAccess.DTO.CartDTOs;
using DataAccess.DTO;

namespace IdentityAjaxClient.Pages.ProductPage.Orchids
{
    public class DetailsModel : PageModel
    {

        private readonly ILogger<DetailsModel> _logger;
        private readonly HttpClient _httpClient;

        public DetailsModel(ILogger<DetailsModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public ProductDTO Product { get; set; } = new();
        public bool IsCustomer = false;
        [BindProperty(SupportsGet = true)]
        public string? ErrorMessage { get; set; }
        public bool IsProductLoaded { get; set; } = false;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            _logger.LogInformation("Details page requested for Product ID: {Id}", id);
            // Check if user is logged in and is a customer
            IsCustomer = HttpContext.Session.GetString("UserRole") == "Customer";

            if (id <= 0)
            {
                ErrorMessage = "Invalid product ID.";
                _logger.LogWarning("Invalid Product ID: {Id}", id);
                return Page();
            }

            try
            {
                var endpoint = $"Products/{id}";
                _logger.LogInformation("Sending GET request to API endpoint: {Endpoint}", endpoint);

                var httpResponse = await _httpClient.GetAsync($"Products/{id}");

                if (httpResponse.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var json = await httpResponse.Content.ReadAsStringAsync();
                    _logger.LogInformation("Raw API response JSON: {Json}", json);

                    var product = JsonSerializer.Deserialize<ProductDTO>(json, options);


                    if (product != null && !string.IsNullOrEmpty(product.ProductName))
                    {
                        Product = product;
                        IsProductLoaded = true;
                    }
                    else
                    {
                        ErrorMessage = "Product not found.";
                        _logger.LogWarning("Product deserialization returned null or incomplete data.");
                    }
                }
                else
                {
                    ErrorMessage = $"API returned status {httpResponse.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch product details.");
                ErrorMessage = "Something went wrong while fetching product details.";
            }

            return Page();
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



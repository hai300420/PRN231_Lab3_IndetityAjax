using BusinessObjects;
using DataAccess.DTO.CartDTOs;
using DataAccess.DTO.OrderDetailDTOs;
using DataAccess.DTO.OrderDTOs;
using DataAccess.DTO.ProductDTOs;
using IdentityAjaxClient.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace IdentityAjaxClient.Pages.CheckoutPage
{
    public class IndexModel : PageModel
    {
        private const string CartSessionKey = "Cart";
        private readonly HttpClient _httpClient;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IHttpClientFactory httpClientFactory, ILogger<IndexModel> logger)
        {
            _httpClient = httpClientFactory.CreateClient("API");
            _logger = logger;
        }

        [BindProperty]
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();

        public decimal TotalPrice { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public Account? UserAccount { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            // Get user account information
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/AuthPage/Login", new { returnUrl = "/CheckoutPage/Index" });
            }

            var accountResponse = await _httpClient.GetAsync($"accounts?idSearch={userId}");

            if (accountResponse.IsSuccessStatusCode)
            {
                var accountList = await accountResponse.Content.ReadFromJsonAsync<PaginationDTO<Account>>();
                UserAccount = accountList?.Items.FirstOrDefault();
            }

            // Get cart info
            var cartJson = HttpContext.Session.GetString(CartSessionKey);

            if (string.IsNullOrEmpty(cartJson))
            {
                CartItems = new List<CartItemViewModel>();
                return Page();
            }

            var cartItems = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartJson);

            if (cartItems == null || !cartItems.Any())
            {
                CartItems = new List<CartItemViewModel>();
                return Page();
            }

            CartItems = new List<CartItemViewModel>();

            foreach (var item in cartItems)
            {
                var productResponse = await _httpClient.GetAsync($"products/{item.OrchidId}");

                if (!productResponse.IsSuccessStatusCode) continue;

                var product = await productResponse.Content.ReadFromJsonAsync<ProductDTO>();

                if (product == null) continue;

                CartItems.Add(new CartItemViewModel
                {
                    ProductId = item.OrchidId,
                    Quantity = item.Quantity,
                    Product = product
                });
            }

            TotalPrice = CartItems.Sum(i => (i.Product?.UnitPrice ?? 0) * i.Quantity);

            return Page();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("UserId not found in session. Redirecting to login.");
                return RedirectToPage("/AuthPage/Login");
            }

            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                _logger.LogWarning("Cart is empty in session.");
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToPage("/CartPage/Index");
            }

            var cartItems = JsonSerializer.Deserialize<List<CartItem>>(cartJson);

            if (cartItems == null || !cartItems.Any())
            {
                _logger.LogWarning("Deserialized cart is null or empty.");
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToPage("/CartPage/Index");
            }

            _logger.LogInformation("Preparing to create order for user {UserId} with {ItemCount} items.", userId, cartItems.Count);

            // Step 1: Create Order
            decimal totalPrice = 0;
            foreach (var item in cartItems)
            {
                totalPrice += item.Quantity * item.Price;    
            }

            var orderDto = new OrderCreateDTO
            {
                AccountId = int.Parse(userId),
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalPrice
            };

            _logger.LogInformation("Sending order creation request to API: {@OrderDto}", orderDto);
            var orderResponse = await _httpClient.PostAsJsonAsync("orders", orderDto);

            if (!orderResponse.IsSuccessStatusCode)
            {
                _logger.LogError("Order creation failed with status code {StatusCode}", orderResponse.StatusCode);
                TempData["ErrorMessage"] = "Failed to place order.";
                return RedirectToPage();
            }

            // Get created orderId (assuming your API returns it in the body)
            var createdOrder = await orderResponse.Content.ReadFromJsonAsync<Order>();
            if (createdOrder == null)
            {
                _logger.LogError("Failed to deserialize created order response.");
                TempData["ErrorMessage"] = "Failed to retrieve order info.";
                return RedirectToPage();
            }

            _logger.LogInformation("Order created successfully with OrderId: {OrderId}", createdOrder.OrderId);
            var orderId = createdOrder.OrderId;

            // Step 2: Create OrderDetails
            bool allDetailsSucceeded = true;

            foreach (var item in cartItems)
            {
                var orderDetailDto = new OrderDetailCreateDTO
                {
                    OrderId = orderId,
                    ProductId = item.OrchidId,
                    Quantity = item.Quantity,
                    Price = item.Price
                };

                _logger.LogInformation("Creating order detail: {@OrderDetailDto}", orderDetailDto);

                var detailResponse = await _httpClient.PostAsJsonAsync("orderdetails", orderDetailDto);
                if (!detailResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to create order detail for ProductId {ProductId}. Status code: {StatusCode}",
                item.OrchidId, detailResponse.StatusCode);
                    allDetailsSucceeded = false;
                    break; // or continue if you want partial success
                }
            }

            if (!allDetailsSucceeded)
            {
                _logger.LogWarning("One or more order details failed to create.");
                TempData["ErrorMessage"] = "Some order details failed. Please try again.";
                return RedirectToPage();
            }

            // Step 3: Clear Cart & Redirect
            _logger.LogInformation("All order details saved successfully. Clearing cart and redirecting.");
            HttpContext.Session.Remove(CartSessionKey);
            return RedirectToPage("/CheckoutPage/Confirmation", new { id = orderId });


        }



    }
}

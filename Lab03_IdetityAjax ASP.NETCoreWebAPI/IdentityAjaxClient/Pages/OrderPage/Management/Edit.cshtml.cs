//using BusinessObjects;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using System.Text.Json;
//using System.Text;
//using DataAccess.DTO.OrderDTOs;

//namespace IdentityAjaxClient.Pages.OrderPage.Management
//{
//    public class EditModel : PageModel
//    {
//        private readonly HttpClient _httpClient;
//        private readonly ILogger<EditModel> _logger;

//        public EditModel(IHttpClientFactory httpClientFactory, ILogger<EditModel> logger)
//        {
//            _httpClient = httpClientFactory.CreateClient("API");
//            _logger = logger;
//        }

//        [BindProperty]
//        public Order Order { get; set; } = default!;

//        [BindProperty]
//        public string OrderStatus { get; set; } = string.Empty;

//        public SelectList OrderStatusOptions { get; set; } = default!;
//        [BindProperty]
//        public OrderUpdateDTO OrderUpdate { get; set; } = new();

//        public string ErrorMessage { get; set; } = string.Empty;
//        public async Task<IActionResult> OnGetAsync(int id)
//        {
//            try
//            {
//                var response = await _httpClient.GetAsync($"orders/{id}");
//                if (!response.IsSuccessStatusCode)
//                {
//                    ErrorMessage = "Failed to fetch order details.";
//                    return Page();
//                }

//                var order = await response.Content.ReadFromJsonAsync<Order>();
//                if (order == null)
//                {
//                    ErrorMessage = "Order not found.";
//                    return RedirectToPage("./Index");
//                }

//                Order = order;
//                OrderStatus = order.OrderStatus ?? string.Empty;
//                OrderUpdate = new OrderUpdateDTO
//                {
//                    AccountId = order.AccountId,
//                    OrderDate = order.OrderDate,
//                    OrderStatus = order.OrderStatus,
//                    TotalAmount = order.TotalAmount
//                };

//                return Page();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error loading order.");
//                ErrorMessage = "An error occurred while loading the order.";
//                return Page();
//            }
//        }

//        public async Task<IActionResult> OnPostAsync()
//        {
//            if (!ModelState.IsValid)
//            {
//                var errors = ModelState.Values
//                    .SelectMany(v => v.Errors)
//                    .Select(e => e.ErrorMessage)
//                    .ToList();

//                TempData["ErrorMessage"] = "Invalid input: " + string.Join(" | ", errors);
//                _logger.LogWarning("Invalid ModelState: {Errors}", errors);
//                return Page();
//            }

//            try
//            {
//                // var response = await _httpClient.PutAsJsonAsync($"Orders/{Order.OrderId}", updateDto);

//                // Send update request
//                var updateDto = new
//                {
//                    AccountId = Order.AccountId,
//                    OrderDate = Order.OrderDate,
//                    OrderStatus = OrderStatus,
//                    TotalAmount = Order.TotalAmount
//                };

//                // var content = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

//                var response = await _httpClient.PutAsJsonAsync($"orders/{Order.OrderId}", OrderUpdate);


//                if (response.IsSuccessStatusCode)
//                {
//                    return RedirectToPage("./Details", new { id = Order.OrderId });
//                }

//                var error = await response.Content.ReadAsStringAsync();
//                return Page();
//            }
//            catch (Exception ex)
//            {
//                ErrorMessage = "An error occurred while updating the order.";
//                return Page();
//            }
//        }
//    }
//}


using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.Text;
using DataAccess.DTO.OrderDTOs;

namespace IdentityAjaxClient.Pages.OrderPage.Management
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<EditModel> _logger;

        public EditModel(IHttpClientFactory httpClientFactory, ILogger<EditModel> logger)
        {
            _httpClient = httpClientFactory.CreateClient("API");
            _logger = logger;
        }

        // Only for display
        public Order Order { get; set; } = default!;

        // Only for form submission
        [BindProperty]
        public OrderUpdateDTO OrderUpdate { get; set; } = new();

        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"orders/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = "Failed to fetch order details.";
                    return Page();
                }

                var order = await response.Content.ReadFromJsonAsync<Order>();
                if (order == null)
                {
                    ErrorMessage = "Order not found.";
                    return RedirectToPage("./Index");
                }

                Order = order;

                // Fill update DTO from the real Order
                OrderUpdate = new OrderUpdateDTO
                {
                    AccountId = order.AccountId,
                    OrderDate = order.OrderDate,
                    OrderStatus = order.OrderStatus,
                    TotalAmount = order.TotalAmount
                };

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading order.");
                ErrorMessage = "An error occurred while loading the order.";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                TempData["ErrorMessage"] = "Invalid input: " + string.Join(" | ", errors);
                _logger.LogWarning("Invalid ModelState: {Errors}", errors);
                return Page();
            }

            try
            {
                var response = await _httpClient.PutAsJsonAsync($"orders/{id}", OrderUpdate);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./Details", new { id });
                }

                var error = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"Failed to update order: {error}";
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order.");
                ErrorMessage = "An error occurred while updating the order.";
                return Page();
            }
        }
    }
}



using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        [BindProperty]
        public Order Order { get; set; } = default!;

        [BindProperty]
        public string OrderStatus { get; set; } = string.Empty;

        public SelectList OrderStatusOptions { get; set; } = default!;

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
                OrderStatus = order.OrderStatus ?? string.Empty;

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading order.");
                ErrorMessage = "An error occurred while loading the order.";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var updateDto = new
                {
                    OrderStatus = OrderStatus
                };

                var response = await _httpClient.PutAsJsonAsync($"orders/{Order.OrderId}", updateDto);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./Details", new { id = Order.OrderId });
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

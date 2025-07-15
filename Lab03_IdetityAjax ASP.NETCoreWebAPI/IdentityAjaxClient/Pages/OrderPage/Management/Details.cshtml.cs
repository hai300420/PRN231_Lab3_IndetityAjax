using BusinessObjects;
using DataAccess.DTO.OrderDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityAjaxClient.Pages.OrderPage.Management
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<IndexModel> _logger;

        public DetailsModel(IHttpClientFactory httpClientFactory, ILogger<IndexModel> logger)
        {
            _httpClient = httpClientFactory.CreateClient("API");
            _logger = logger;
        }

        public Order Order { get; set; } = default!;
        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"orders/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = $"Unable to retrieve order #{id}.";
                    return Page();
                }

                var order = await response.Content.ReadFromJsonAsync<Order>();

                if (order == null)
                {
                    ErrorMessage = "Order not found.";
                    return RedirectToPage("./Index");
                }

                Order = order;
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load order details.");
                ErrorMessage = "An error occurred while loading the order.";
                return Page();
            }
        }
    }
}

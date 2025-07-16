using BusinessObjects;
using DataAccess.DTO;
using DataAccess.DTO.OrderDTOs;
using DataAccess.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace IdentityAjaxClient.Pages.OrderPage.Management
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IHttpClientFactory httpClientFactory, ILogger<IndexModel> logger)
        {
            _httpClient = httpClientFactory.CreateClient("API");
            _logger = logger;
        }
        public bool IsStaff { get; private set; }

        public PagedResultDetail<OrderDTO> Orders { get; set; } = default!;
        public SelectList? OrderStatusList { get; set; }

        // Search and filter properties
        [BindProperty(SupportsGet = true)]
        public OrderStatusEnum? OrderStatusSearch { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? CustomerSearch { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDateSearch { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDateSearch { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        private const int PageSize = 10;

        public async Task<IActionResult> OnGetAsync()
        {
            // Get role
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");

            IsStaff = !string.IsNullOrEmpty(userRole) &&
                    (userRole.Equals("Staff", StringComparison.OrdinalIgnoreCase));

            bool isCustomer = !string.IsNullOrEmpty(userRole) &&
                (userRole.Equals("Customer", StringComparison.OrdinalIgnoreCase));

            // Check authorization
            if (!IsStaff && !isCustomer)
            {
                TempData["ErrorMessage"] = "You are not authorized to access order management.";
                return RedirectToPage("/Index");
            }

            // 1. Build the query string for filtering
            var queryParams = new Dictionary<string, string?>
            {
                ["page"] = PageIndex.ToString(),
                ["pageSize"] = PageSize.ToString(),
                ["orderStatus"] = OrderStatusSearch?.ToString(),
                ["customer"] = CustomerSearch,
                ["startDate"] = StartDateSearch?.ToString("yyyy-MM-dd"),
                ["endDate"] = EndDateSearch?.ToString("yyyy-MM-dd")
            };
            if (isCustomer && !string.IsNullOrEmpty(userId))
            {
                queryParams["accountId"] = userId; // Customers view his order
            }

            string queryString = string.Join("&", queryParams
                .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
                .Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value!)}"));

            var response = await _httpClient.GetAsync($"orders?{queryString}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to load orders.";
                return Page();
            }

            var json = await response.Content.ReadAsStringAsync();
            Orders = JsonSerializer.Deserialize<PagedResultDetail<OrderDTO>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

            // 2. Setup status dropdown list
            OrderStatusList = new SelectList(
                Enum.GetValues(typeof(OrderStatusEnum))
                    .Cast<OrderStatusEnum>()
                    .Select(s => new SelectListItem
                    {
                        Value = s.ToString(),
                        Text = s.ToString()
                    }), "Value", "Text");


            return Page();
        }
    }
}

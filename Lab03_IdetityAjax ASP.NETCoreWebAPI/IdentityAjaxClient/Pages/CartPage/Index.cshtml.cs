using BusinessObjects;
using DataAccess.DTO.CartDTOs;
using DataAccess.DTO.ProductDTOs;
using IdentityAjaxClient.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityAjaxClient.Pages.CartPage
{
    public class IndexModel : PageModel
    {
        private const string CartSessionKey = "Cart";

        private readonly HttpClient _httpClient;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }

        [BindProperty]
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();

        public decimal TotalPrice { get; set; }

        public async Task OnGetAsync()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);

            if (string.IsNullOrEmpty(cartJson))
            {
                CartItems = new List<CartItemViewModel>();
                return;
            }

            var cartItems = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartJson);

            if (cartItems == null || !cartItems.Any())
            {
                CartItems = new List<CartItemViewModel>();
                return;
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
        }

    }
}

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
using Microsoft.SqlServer.Server;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Metadata;

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


//#region UI
//@page "{id:int}"
//@model IdentityAjaxClient.Pages.ProductPage.Orchids.DetailsModel
//@{
//    ViewData["Title"] = "Orchid Details";
//}

//<div class="container py-5">
//    @if (!string.IsNullOrEmpty(Model.ErrorMessage))
//    {
//        <div class="alert alert-danger">
//            <strong>Error:</strong> @Model.ErrorMessage
//        </div>
//    }
//    else if (!Model.IsProductLoaded)
//    {
//        <div class="alert alert-info">
//            Loading product details...
//        </div>
//    }
//    else
//    {
//        <div class="row">
//            <div class="col-md-6">
//                @if (!string.IsNullOrEmpty(Model.Product.ProductUrl))
//                {
//                    <img src="@Model.Product.ProductUrl" alt="@Model.Product.ProductName" class="img-fluid rounded shadow" />
//                }
//                else
//                {
//                    <div class="bg-light d-flex justify-content-center align-items-center" style="height: 300px;">
//                        <span class="text-muted">No image available</span>
//                    </div>
//                }
//            </div>
//            <div class="col-md-6">
//                <h2>@Model.Product.ProductName</h2>
//                <p class="text-muted">@Model.Product.CategoryName</p>

//                <h4 class="text-danger">@Model.Product.UnitPrice.ToString("C", new System.Globalization.CultureInfo("en-US"))</h4>

//                <p class="mt-3">
//                    @if (string.IsNullOrEmpty(Model.Product.ProductDescription))
//                    {
//                        <em>No description available.</em>
//                    }
//                    else
//                    {
//                    <p>Description: @Model.Product.ProductDescription</p>
//                    }
//                </p>

//                <p><strong>Type:</strong> @(Model.Product.IsNatural == true ? "Natural" : "Hybrid")</p>
//            </div>
//            @if (Model.IsCustomer)
//            {
//                <form id="addToCartForm" method="post" asp-page-handler="AddToCart">
//                    <input type="hidden" name="orchidId" value="@Model.Product.ProductId" />
//                    <div class="row g-3 mb-4">
//                        <div class="col-md-4">
//                            <label class="form-label mb-2">Quantity</label>
//                            <div class="quantity-control d-flex">
//                                <button type="button" class="btn quantity-btn" onclick="decreaseQuantity()">
//                                    <i class="bi bi-dash"></i>
//                                </button>
//                                <div class="quantity-input-wrapper">
//                                    <input type="number" id="quantity" name="quantity" class="form-control quantity-input" value="1" min="1" max="10">
//                                </div>
//                                <button type="button" class="btn quantity-btn" onclick="increaseQuantity()">
//                                    <i class="bi bi-plus"></i>
//                                </button>
//                            </div>
//                        </div>
//                        <div class="col-md-8 d-grid">
//                            <button type="submit" id="addToCartBtn" class="btn btn-primary btn-lg hero-btn">
//                                <i class="bi bi-cart-plus me-2"></i> Add to Cart
//                            </button>
//                        </div>
//                    </div>
//                </form>
//            }
//            <a asp-page="/Index" class="btn btn-outline-primary mt-3">
//                <i class="bi bi-arrow-left me-2"></i> Browse More Orchids
//            </a>

//        </div>
//    }
//</div>
//<form id="antiforgery" method="post">
//    @Html.AntiForgeryToken()
//</form>

//@section Scripts {
//    <script>
//        function increaseQuantity() {
//            var input = document.getElementById('quantity');
//            var value = parseInt(input.value, 10);
//            if (value < parseInt(input.max)) {
//                input.value = value + 1;
//            }
//        }

//        function decreaseQuantity() {
//            var input = document.getElementById('quantity');
//            var value = parseInt(input.value, 10);
//            if (value > parseInt(input.min)) {
//        input.value = value - 1;
//    }
//        }

//        // Add to cart AJAX functionality
//        document.addEventListener('DOMContentLoaded', function() {
//            const addToCartForm = document.getElementById('addToCartForm');

//            if (addToCartForm) {
//                addToCartForm.addEventListener('submit', function(e) {
//                    e.preventDefault();

//                    const orchidId = this.querySelector('input[name="orchidId"]').value;
//                    const quantity = document.getElementById('quantity').value;

//                    const form = new FormData();
//            form.append('orchidId', orchidId);
//                    form.append('quantity', quantity);

//                    // Show loading state
//                    const submitBtn = document.getElementById('addToCartBtn');
//                    const originalText = submitBtn.innerHTML;
//                    submitBtn.disabled = true;
//                    submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Adding...';

//                    // Get the anti-forgery token
//                    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

//                    fetch('?handler=AddToCart', {
//                        method: 'POST',
//                        body: form,
//                        headers: {
//                            'RequestVerificationToken': token
//                        }
//                    })
//                    .then(response => {
//                        if (!response.ok) {
//                            throw new Error('Network response was not ok');
//                        }
//                        return response.json();
//                    })
//                    .then(data => {
//                        // Reset button
//                        submitBtn.disabled = false;
//                        submitBtn.innerHTML = originalText;

//                        // Show toast notification
//                        if (data.success) {
//                            ToastManager.success(data.message || 'Item added to cart successfully!');
//                        } else {
//                            ToastManager.error(data.message || 'Failed to add item to cart.');
//                        }
//                    })
//                    .catch(error => {
//                        console.error('Error:', error);
//                        submitBtn.disabled = false;
//                        submitBtn.innerHTML = originalText;
//                        ToastManager.error('An error occurred while adding to cart.');
//                    });
//                });
//            }
//        });
//    </script>
//}

//@section Styles {
//    <style>
//        .text-danger {
//            color: #e74c3c;
//        }
//    </style>
//}

//@page "{id:int}"
//@model IdentityAjaxClient.Pages.ProductPage.Orchids.DetailsModel
//@{
//    ViewData["Title"] = "Orchid Details";
//}

//<div class="container py-5">
//    @if (!string.IsNullOrEmpty(Model.ErrorMessage))
//    {
//        <div class="alert alert-danger">
//            <strong>Error:</strong> @Model.ErrorMessage
//        </div>
//    }
//    else if (!Model.IsProductLoaded)
//    {
//        <div class="alert alert-info">
//            Loading product details...
//        </div>
//    }
//    else
//    {
//        <div class="row">
//            <div class="col-md-6">
//                @if (!string.IsNullOrEmpty(Model.Product.ProductUrl))
//                {
//                    <img src="@Model.Product.ProductUrl" alt="@Model.Product.ProductName" class="img-fluid rounded shadow" />
//                }
//                else
//                {
//                    <div class="bg-light d-flex justify-content-center align-items-center" style="height: 300px;">
//                        <span class="text-muted">No image available</span>
//                    </div>
//                }
//            </div>
//            <div class="col-md-6">
//                <h2>@Model.Product.ProductName</h2>
//                <p class="text-muted">@Model.Product.CategoryName</p>

//                <h4 class="text-danger">@Model.Product.UnitPrice.ToString("C", new System.Globalization.CultureInfo("en-US"))</h4>

//                <p class="mt-3">
//                    @if (string.IsNullOrEmpty(Model.Product.ProductDescription))
//                    {
//                        <em>No description available.</em>
//                    }
//                    else
//                    {
//                    <p>Description: @Model.Product.ProductDescription</p>
//                    }
//                </p>

//                <p><strong>Type:</strong> @(Model.Product.IsNatural == true ? "Natural" : "Hybrid")</p>
//            </div>
//            @if (Model.IsCustomer)
//            {
//                <form id="addToCartForm" method="post" asp-page-handler="AddToCart">
//                    <input type="hidden" name="orchidId" value="@Model.Product.ProductId" />
//                    <div class="row g-3 mb-4">
//                        <div class="col-md-4">
//                            <label class="form-label mb-2">Quantity</label>
//                            <div class="quantity-control d-flex">
//                                <button type="button" class="btn quantity-btn" onclick="decreaseQuantity()">
//                                    <i class="bi bi-dash"></i>
//                                </button>
//                                <div class="quantity-input-wrapper">
//                                    <input type="number" id="quantity" name="quantity" class="form-control quantity-input" value="1" min="1" max="10">
//                                </div>
//                                <button type="button" class="btn quantity-btn" onclick="increaseQuantity()">
//                                    <i class="bi bi-plus"></i>
//                                </button>
//                            </div>
//                        </div>
//                        <div class="col-md-8 d-grid">
//                            <button type="submit" id="addToCartBtn" class="btn btn-primary btn-lg hero-btn">
//                                <i class="bi bi-cart-plus me-2"></i> Add to Cart
//                            </button>
//                        </div>
//                    </div>
//                </form>
//            }
//            <a asp-page="/Index" class="btn btn-outline-primary mt-3">
//                <i class="bi bi-arrow-left me-2"></i> Browse More Orchids
//            </a>

//        </div>
//    }
//</div>
//<form id="antiforgery" method="post">
//    @Html.AntiForgeryToken()
//</form>

//@section Scripts {
//    <script>
//        function increaseQuantity() {
//            var input = document.getElementById('quantity');
//            var value = parseInt(input.value, 10);
//if (value < parseInt(input.max)) {
//                input.value = value + 1;
//            }
//        }

//        function decreaseQuantity() {
//            var input = document.getElementById('quantity');
//            var value = parseInt(input.value, 10);
//    if (value > parseInt(input.min)) {
//                input.value = value - 1;
//    }
//}

//// Add to cart AJAX functionality
//        document.addEventListener('DOMContentLoaded', function() {
//            const addToCartForm = document.getElementById('addToCartForm');

//            if (addToCartForm) {
//                addToCartForm.addEventListener('submit', function(e) {
//            e.preventDefault();

//                    const orchidId = this.querySelector('input[name="orchidId"]').value;
//                    const quantity = document.getElementById('quantity').value;

//                    const form = new FormData();
//                    form.append('orchidId', orchidId);
//            form.append('quantity', quantity);

//            // Show loading state
//                    const submitBtn = document.getElementById('addToCartBtn');
//                    const originalText = submitBtn.innerHTML;
//                    submitBtn.disabled = true;
//                    submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Adding...';

//            // Get the anti-forgery token
//                    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

//                    fetch('?handler=AddToCart', {
//                        method: 'POST',
//                        body: form,
//                        headers: {
//                            'RequestVerificationToken': token
//                        }
//                    })
//                    .then(response => {
//                        if (!response.ok) {
//                            throw new Error('Network response was not ok');
//                        }
//                        return response.json();
//                    })
//                    .then(data => {
//                        // Reset button
//                        submitBtn.disabled = false;
//                        submitBtn.innerHTML = originalText;

//                        // Show toast notification
//                        if (data.success) {
//                            ToastManager.success(data.message || 'Item added to cart successfully!');
//                        } else {
//                            ToastManager.error(data.message || 'Failed to add item to cart.');
//                        }
//                    })
//                    .catch(error => {
//                        console.error('Error:', error);
//                        submitBtn.disabled = false;
//                        submitBtn.innerHTML = originalText;
//                        ToastManager.error('An error occurred while adding to cart.');
//                    });
//                });
//            }
//        });
//    </script>
//}

//@section Styles {
//    <style>
//        .text-danger {
//            color: #e74c3c;
//        }
//    </style>
//}


//#endregion
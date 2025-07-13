using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataAccess.DTO.ProductDTOs;
using System.Text.Json;
using Azure;
using BusinessObjects;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.CompilerServices;
using System;

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

        public string? ErrorMessage { get; set; }
        public bool IsProductLoaded { get; set; } = false;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            _logger.LogInformation("Details page requested for Product ID: {Id}", id);

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


    }
}



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
//                        @Model.Product.ProductDescription
//                    }
//                </p>

//                <p><strong>Type:</strong> @(Model.Product.IsNatural == true ? "Natural" : "Hybrid")</p>
//            </div>
//        </div>
//    }
//</div>
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
//                        @Model.Product.ProductDescription
//                    }
//                </p>

//                <p><strong>Type:</strong> @(Model.Product.IsNatural == true ? "Natural" : "Hybrid")</p>
//            </div>
//        </div>
//    }
//</div>

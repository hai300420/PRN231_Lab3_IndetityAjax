using DataAccess.DTO.CategoryDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;

namespace IdentityAjaxClient.Pages.CategoryPage.Management
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(IHttpClientFactory httpClientFactory, ILogger<CreateModel> logger)
        {
            _httpClient = httpClientFactory.CreateClient("API");
            _logger = logger;
        }

        [BindProperty]
        public CategoryCreateDTO Category { get; set; } = default!;

        [TempData]
        public string StatusMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                StatusMessage = "Error: Please correct the form.";
                return Page();
            }

            try
            {
                var json = JsonSerializer.Serialize(Category);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Categories", content);

                if (response.IsSuccessStatusCode)
                {
                    StatusMessage = "Category created successfully!";
                    return RedirectToPage("./Index");
                }
                else
                {
                    StatusMessage = "Error: Failed to create category.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category.");
                StatusMessage = "Error: An unexpected error occurred.";
                return Page();
            }
        }
    }
}

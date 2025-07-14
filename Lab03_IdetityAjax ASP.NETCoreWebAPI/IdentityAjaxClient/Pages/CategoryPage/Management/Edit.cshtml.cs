using DataAccess.DTO.CategoryDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;

namespace IdentityAjaxClient.Pages.CategoryPage.Management
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
        public CategoryUpdateDTO CategoryInput { get; set; } = default!;

        [TempData]
        public string StatusMessage { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Categories/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    StatusMessage = "Error: Unable to load category.";
                    return RedirectToPage("./Index");
                }

                var category = await response.Content.ReadFromJsonAsync<CategoryUpdateDTO>();
                if (category == null)
                {
                    StatusMessage = "Error: Category data not found.";
                    return RedirectToPage("./Index");
                }

                CategoryInput = category;
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load category.");
                StatusMessage = "Error: An unexpected error occurred.";
                return RedirectToPage("./Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                StatusMessage = "Error: Please correct the form.";
                return Page();
            }

            try
            {
                var json = JsonSerializer.Serialize(CategoryInput);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"Categories/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    StatusMessage = "Category updated successfully!";
                    return RedirectToPage("./Index");
                }
                else
                {
                    StatusMessage = "Error: Failed to update category.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category.");
                StatusMessage = "Error: An unexpected error occurred.";
                return Page();
            }
        }

    }
}

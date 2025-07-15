using BusinessObjects;
using DataAccess.DTO.AccountDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityAjaxClient.Pages.UserPage.Management
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;

        private const string ADMIN_ROLE = "Admin";

        public DetailsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public AccountDTO Account { get; set; } = default!;

        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"accounts/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = $"Failed to load user with ID {id}.";
                    return RedirectToPage("./Index");
                }

                var account = await response.Content.ReadFromJsonAsync<AccountDTO>();

                if (account == null)
                {
                    ErrorMessage = $"User with ID {id} not found.";
                    return RedirectToPage("./Index");
                }

                Account = account;
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
                return RedirectToPage("./Index");
            }
        }
    }
}

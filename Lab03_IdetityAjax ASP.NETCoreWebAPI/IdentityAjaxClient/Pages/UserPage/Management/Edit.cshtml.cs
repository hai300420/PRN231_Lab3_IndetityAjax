using DataAccess.DTO.AccountDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace IdentityAjaxClient.Pages.UserPage.Management
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
        public AccountUpdateDTO Account { get; set; } = default!;

        // Store the role name for display purposes only
        public string? RoleName { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var response = await _httpClient.GetAsync($"accounts/{Id}");

            if (!response.IsSuccessStatusCode)
                return RedirectToPage("./Index");

            var dto = await response.Content.ReadFromJsonAsync<AccountDTO>();

            if (dto == null)
                return RedirectToPage("./Index");

            Account = new AccountUpdateDTO
            {
                AccountName = dto.AccountName,
                Email = dto.Email,
                RoleId = dto.RoleId,
                Password = string.Empty,
                RoleName = dto.RoleName
            };

            

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid.");
                return Page();
            }

            _logger.LogInformation("Sending PUT request to update account with ID {Id}", Id);
            _logger.LogInformation("Payload: {@Account}", Account);

            var response = await _httpClient.PutAsJsonAsync($"accounts/{Id}", Account);

            _logger.LogInformation("Payload: {@Account}", Account);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to update account. StatusCode: {StatusCode}. Response: {Error}", response.StatusCode, error);
                ModelState.AddModelError(string.Empty, "Failed to update the user.");
                return Page();
            }
            _logger.LogInformation("Account update succeeded.");
            return RedirectToPage("./Index");
        }
    }
}

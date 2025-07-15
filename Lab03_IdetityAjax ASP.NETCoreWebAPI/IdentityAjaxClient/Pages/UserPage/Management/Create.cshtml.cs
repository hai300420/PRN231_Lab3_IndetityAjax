using DataAccess.DTO.AccountDTOs;
using DataAccess.DTO.RoleDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityAjaxClient.Pages.UserPage.Management
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

        public SelectList? RoleList { get; set; }

        [BindProperty]
        public AccountCreateDTO Account { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync()
        {
            var response = await _httpClient.GetAsync("roles");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to load roles. Status code: {StatusCode}", response.StatusCode);
                RoleList = new SelectList(Enumerable.Empty<SelectListItem>());
                return Page();
            }

            var roles = await response.Content.ReadFromJsonAsync<List<RoleDTO>>();
            RoleList = new SelectList(roles, "RoleId", "RoleName");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid.");
                await LoadRolesAsync();
                return Page();
            }

            _logger.LogInformation("Creating account: {@Account}", Account);

            var response = await _httpClient.PostAsJsonAsync("accounts", Account);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to create account. Response: {Error}", error);
                ModelState.AddModelError(string.Empty, "Failed to create the user.");
                await LoadRolesAsync();
                return Page();
            }

            _logger.LogInformation("Account created successfully.");
            return RedirectToPage("./Index");
        }

        private async Task LoadRolesAsync()
        {
            var response = await _httpClient.GetAsync("roles");

            if (response.IsSuccessStatusCode)
            {
                var roles = await response.Content.ReadFromJsonAsync<List<RoleDTO>>();
                RoleList = new SelectList(roles, "RoleId", "RoleName");
            }
            else
            {
                RoleList = new SelectList(Enumerable.Empty<SelectListItem>());
            }
        }
    }
}

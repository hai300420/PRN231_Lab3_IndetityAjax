using DataAccess.DTO.AccountDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityAjaxClient.Pages.AuthPage
{
    public class RegisterModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RegisterModel> _logger;

        [BindProperty]
        public RegisterDTO RegisterDTO { get; set; } = new();

        public RegisterModel(IHttpClientFactory httpClientFactory, ILogger<RegisterModel> logger)
        {
            _httpClient = httpClientFactory.CreateClient("API");
            _logger = logger;
        }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model validation failed");
                return Page();
            }

            var registerRequest = new
            {
                AccountName = RegisterDTO.AccountName,
                Email = RegisterDTO.Email,
                Password = RegisterDTO.Password
            };

            _logger.LogInformation("Sending registration request: {@Register}", registerRequest);

            var response = await _httpClient.PostAsJsonAsync("auth/register", registerRequest);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Registration failed: {StatusCode} - {Error}", response.StatusCode, error);
                ModelState.AddModelError(string.Empty, "Registration failed. " + error);
                return Page();
            }

            _logger.LogInformation("Registration successful. Redirecting to login.");
            return RedirectToPage("/AuthPage/Login");
        }
    }
}

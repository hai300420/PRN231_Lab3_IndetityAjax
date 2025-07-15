using DataAccess.DTO.AccountDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IdentityAjaxClient.Pages.AuthPage
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(IHttpClientFactory httpClientFactory, ILogger<LoginModel> logger)
        {
            _httpClient = httpClientFactory.CreateClient("API");
            _logger = logger;
        }

        [BindProperty]
        public LoginRequest LoginDTO { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var response = await _httpClient.PostAsJsonAsync("auth/login", LoginDTO);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result is null || string.IsNullOrEmpty(result.Token))
            {
                ModelState.AddModelError(string.Empty, "Login failed. Please try again.");
                return Page();
            }

            // Save JWT to session
            HttpContext.Session.SetString("JWTToken", result.Token);

            // Decode JWT to extract role & accountId
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(result.Token);

            var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "customer";
            var accountId = jwtToken.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value ?? "";

            HttpContext.Session.SetString("UserRole", role);
            HttpContext.Session.SetString("UserId", accountId);

            return RedirectToPage("/Index"); // or any secure page
        }

        private class LoginResponse
        {
            public string Token { get; set; } = string.Empty;
        }


    }
}

using BusinessObjects;
using DataAccess.DTO;
using DataAccess.DTO.AccountDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityAjaxClient.Pages.UserPage.Management
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public PagedResultDetail<Account> Account { get; set; } = default!;
        public SelectList? RoleList { get; set; }

        // Search and filter properties
        [BindProperty(SupportsGet = true)]
        public string? NameSearch { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? EmailSearch { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? RoleIdSearch { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Build query string
                var query = $"?page={PageIndex}&pageSize=10";

                if (!string.IsNullOrWhiteSpace(NameSearch))
                    query += $"&name={Uri.EscapeDataString(NameSearch)}";

                if (!string.IsNullOrWhiteSpace(EmailSearch))
                    query += $"&email={Uri.EscapeDataString(EmailSearch)}";

                if (RoleIdSearch.HasValue)
                    query += $"&roleId={RoleIdSearch.Value}";

                // Call accounts API
                var accountResponse = await _httpClient.GetFromJsonAsync<PagedResultDetail<Account>>($"accounts{query}");

                if (accountResponse == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to load user list.");
                    Account = new PagedResultDetail<Account>();
                }
                else
                {
                    Account = accountResponse;
                }

                // Load roles for filter
                var rolesResponse = await _httpClient.GetFromJsonAsync<List<Role>>("roles");
                if (rolesResponse != null)
                {
                    RoleList = new SelectList(rolesResponse, "RoleId", "RoleName");
                }

                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                Account = new PagedResultDetail<Account>();
                return Page();
            }
        }
    }
}

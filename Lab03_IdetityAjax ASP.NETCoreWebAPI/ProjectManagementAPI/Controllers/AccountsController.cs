using BusinessObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interface;
using Repositories;
using DataAccess.DTO.AccountDTOs;
using DataAccess.DTO.CategoryDTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Client;
using DataAccess.DTO;

namespace ProjectManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository repository = new AccountRepository();

        //[HttpGet]
        //public ActionResult<IEnumerable<Account>> GetAccounts()
        //{
        //    var accounts = repository.GetAccounts();
        //    var result = accounts.Select(c => new AccountDTO
        //    {
        //        AccountId = c.AccountId,
        //        AccountName = c.AccountName,
        //        Email = c.Email,
        //        RoleId = c.RoleId,
        //        RoleName = c.Role?.RoleName
        //    });

        //    return Ok(result);
        //}

        //[HttpGet("{id}")]
        //public ActionResult<Account> GetAccount(int id)
        //{
        //    var account = repository.GetAccountById(id);
        //    return account == null ? NotFound() : Ok(account);
        //}

        [HttpGet]
        public ActionResult<PagedResult<AccountDTO>> GetAccounts(
        int? id = null,
        string? email = null,
        string? name = null,
        int? roleId = null,
        int page = 1,
        int pageSize = 10)
        {
            var accounts = repository.GetAccounts();

            // Filtering
            if (id.HasValue)
            {
                accounts = accounts.Where(a => a.AccountId == id.Value).ToList();
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                accounts = accounts.Where(a =>
                    a.Email != null &&
                    a.Email.Contains(email, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                accounts = accounts.Where(a =>
                    a.AccountName != null &&
                    a.AccountName.Contains(name, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (roleId.HasValue)
            {
                accounts = accounts.Where(a => a.RoleId == roleId.Value).ToList();
            }

            var totalCount = accounts.Count();

            var items = accounts
                .OrderBy(a => a.AccountName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AccountDTO
                {
                    AccountId = a.AccountId,
                    AccountName = a.AccountName,
                    Email = a.Email,
                    RoleId = a.RoleId,
                    RoleName = a.Role?.RoleName
                })
                .ToList();

            return Ok(new PagedResultDetail<AccountDTO>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            });
        }
        
        [HttpGet("{id}")]
        public ActionResult<AccountDTO> GetAccount(int id)
        {
            var a = repository.GetAccountById(id);
            if (a == null) return NotFound();

            var dto = new AccountDTO
            {
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                Email = a.Email,
                RoleId = a.RoleId,
                RoleName = a.Role?.RoleName
            };

            return Ok(dto);
        }

        [HttpPost]
        public IActionResult PostAccount([FromBody] AccountCreateDTO dto)
        {
            var account = new Account
            {
                AccountName = dto.AccountName,
                Email = dto.Email,
                Password = dto.Password,
                RoleId = dto.RoleId
            };

            repository.SaveAccount(account);
            return Ok(new
            {
                Status = "Success",
                Message = "Account created successfully."
            });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAccount(int id, [FromBody] AccountUpdateDTO dto)
        {
            var acc = repository.GetAccountById(id);
            if (acc == null) return NotFound();

            acc.AccountName = dto.AccountName;
            acc.Email = dto.Email;
            // acc.Password = dto.Password;
            if (string.IsNullOrEmpty(dto.Password))
            {
                acc.Password = acc.Password;
            }
            acc.RoleId = dto.RoleId;

            repository.UpdateAccount(acc);

            return Ok(new
            {
                Status = "Success",
                Message = "Account updated successfully."
            });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAccount(int id)
        {
            var acc = repository.GetAccountById(id);
            if (acc == null) return NotFound();

            repository.DeleteAccount(id);
            return Ok(new
            {
                Status = "Success",
                Message = "Account deleted successfully."
            });
        }
    }
}

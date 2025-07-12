using BusinessObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interface;
using Repositories;
using DataAccess.DTO.AccountDTOs;
using DataAccess.DTO.CategoryDTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Client;

namespace ProjectManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository repository = new AccountRepository();

        [HttpGet]
        public ActionResult<IEnumerable<Account>> GetAccounts()
        {
            var accounts = repository.GetAccounts();
            var result = accounts.Select(c => new AccountDTO
            {
                AccountId = c.AccountId,
                AccountName = c.AccountName,
                Email = c.Email,
                RoleId = c.RoleId,
                RoleName = c.Role?.RoleName
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public ActionResult<Account> GetAccount(int id)
        {
            var account = repository.GetAccountById(id);
            return account == null ? NotFound() : Ok(account);
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
            acc.Password = dto.Password;
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

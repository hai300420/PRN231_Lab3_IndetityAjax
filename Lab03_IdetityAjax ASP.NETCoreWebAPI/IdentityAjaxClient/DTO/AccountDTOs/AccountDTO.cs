using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.AccountDTOs
{
    public class AccountDTO
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
    }
}

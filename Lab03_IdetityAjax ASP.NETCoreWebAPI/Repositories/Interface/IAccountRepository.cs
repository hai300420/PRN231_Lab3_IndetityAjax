using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IAccountRepository
    {
        #region CRUD
        List<Account> GetAccounts();
        Account GetAccountById(int id);
        void SaveAccount(Account account);
        void UpdateAccount(Account account);
        void DeleteAccount(int id);
        #endregion

        Account? GetAccountByEmail(string email);
        Account? ValidateLogin(string email, string password);

    }
}

using BusinessObjects;
using DataAccess.DAO;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class AccountRepository : IAccountRepository
    {
        #region CRUD
        public void DeleteAccount(int id) => AccountDAO.DeleteAccount(id);

        public Account GetAccountById(int id) => AccountDAO.GetAccountById(id);

        public List<Account> GetAccounts() => AccountDAO.GetAccounts();

        public void SaveAccount(Account account) => AccountDAO.SaveAccount(account);

        public void UpdateAccount(Account account) => AccountDAO.UpdateAccount(account);
        #endregion

        public Account? GetAccountByEmail(string email) => AccountDAO.GetAccountByEmail(email);

        public Account? ValidateLogin(string email, string password) => AccountDAO.ValidateLogin(email, password);
    }

}


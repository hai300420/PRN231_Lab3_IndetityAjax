using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class AccountDAO
    {
        #region CRUD
        public static List<Account> GetAccounts()
        {
            try
            {
                using var context = new MyStoreDbContext();
                return context.Accounts.Include(a => a.Role).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static Account? GetAccountById(int id)
        {
            try
            {
                using var context = new MyStoreDbContext();
                return context.Accounts.Include(r => r.Role).FirstOrDefault(a => a.AccountId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void SaveAccount(Account account)
        {
            try
            {
                using var context = new MyStoreDbContext();
                context.Accounts.Add(account);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //public static void UpdateAccount(Account account)
        //{
        //    try
        //    {
        //        using var context = new MyStoreDbContext();
        //        context.Entry(account).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        //        context.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        public static void UpdateAccount(Account updatedAccount)
        {
            try
            {
                using var context = new MyStoreDbContext();
                var existing = context.Accounts.FirstOrDefault(a => a.AccountId == updatedAccount.AccountId);
                if (existing == null) throw new Exception("Account not found.");

                existing.AccountName = updatedAccount.AccountName;
                existing.Email = updatedAccount.Email;

                if (!string.IsNullOrEmpty(updatedAccount.Password))
                    existing.Password = updatedAccount.Password;

                existing.RoleId = updatedAccount.RoleId;

                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("EF Core SaveChanges failed.");
                Console.WriteLine("Message: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
                }
                throw;
            }
        }


        public static void DeleteAccount(int id)
        {
            try
            {
                using var context = new MyStoreDbContext();
                var acc = context.Accounts.FirstOrDefault(a => a.AccountId == id);
                if (acc != null)
                {
                    context.Accounts.Remove(acc);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        public static Account? GetAccountByEmail(string email)
        {
            try
            {
                using var context = new MyStoreDbContext();
                return context.Accounts.FirstOrDefault(a => a.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static Account? ValidateLogin(string email, string password)
        {
            try
            {
                using var context = new MyStoreDbContext();
                return context.Accounts.FirstOrDefault(a => a.Email == email && a.Password == password);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}

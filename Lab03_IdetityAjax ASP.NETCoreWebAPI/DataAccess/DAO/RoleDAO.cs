using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class RoleDAO
    {
        public static List<Role> GetRoles()
        {
            try
            {
                using var context = new MyStoreDbContext();
                return context.Roles.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static Role? GetRoleById(int id)
        {
            try
            {
                using var context = new MyStoreDbContext();
                return context.Roles.FirstOrDefault(r => r.RoleId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void SaveRole(Role role)
        {
            try
            {
                using var context = new MyStoreDbContext();
                context.Roles.Add(role);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void UpdateRole(Role role)
        {
            try
            {
                using var context = new MyStoreDbContext();
                context.Entry(role).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void DeleteRole(int id)
        {
            try
            {
                using var context = new MyStoreDbContext();
                var r = context.Roles.FirstOrDefault(x => x.RoleId == id);
                if (r != null)
                {
                    context.Roles.Remove(r);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

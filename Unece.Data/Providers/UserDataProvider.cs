using Unece.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Unece.Data.Providers
{
    public interface IUserDataProvider
    {
        List<USR_Users> GetUsers(bool includeAdminUsers = false);
        void SaveUser(USR_Users user);
        void UpdateUserStatus(int id, bool deleted);
       
       
    }
    public class UserDataProvider : IUserDataProvider
    {
        private readonly UNECEDbContext _context;

        public UserDataProvider(UNECEDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Get All Users
        /// </summary>
        /// <param name="includeAdminUsers"></param>
        /// <returns></returns>
        public List<USR_Users> GetUsers(bool includeAdminUsers = false)
        {
            return _context.USR_Users
                .Where(x => includeAdminUsers || x.IsAdmin == includeAdminUsers)
                .OrderBy(x => x.UserName)
                .ToList(); 
        }
        /// <summary>
        /// Save User details
        /// </summary>
        /// <param name="user">User object</param>
        public void SaveUser(USR_Users user)
        {
            var userUpdate = _context.USR_Users.SingleOrDefault(x => x.Id == user.Id);
            if (userUpdate == null)
                _context.Add(user);
            else
            {
                userUpdate.UserName = user.UserName;
                userUpdate.Password = user.Password;
                userUpdate.IsDeleted = user.IsDeleted;
            }
            _context.SaveChanges();
        }
        /// <summary>
        /// Update User Status
        /// </summary>
        /// <param name="id">User Id</param>
        /// <param name="deleted">delete</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void UpdateUserStatus(int id, bool deleted)
        {
            var userToDelete = _context.USR_Users.SingleOrDefault(x => x.Id == id) ?? throw new InvalidOperationException();
            userToDelete.IsDeleted = deleted;
            _context.SaveChanges();
        }

      

    }
}

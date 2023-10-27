using Unece.Data.Helpers;
using Unece.Data.Models;
using System.Linq;

namespace Unece.Data.Services
{

    public interface IUserAuthenticationService
    {
        bool TryGetLoginUser(USR_Users userLogin, out USR_Users user);
    }
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly UNECEDbContext _context;
        public UserAuthenticationService(UNECEDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// This function is used to check the login details are valid
        /// </summary>
        /// <param name="userLogin"></param>
        /// <param name="user"></param>
        /// <returns>bool is valid thue else false</returns>
        public bool TryGetLoginUser(USR_Users userLogin, out USR_Users user)
        {
            user = null;

            if (userLogin != null &&
                !string.IsNullOrEmpty(userLogin.UserName) &&
                !string.IsNullOrEmpty(userLogin.Password))
            {
                user = _context.USR_Users.SingleOrDefault(u => u.UserName == userLogin.UserName);
                if (user != null && PasswordHelper.VerifyEncryptedPassword(user.Password, userLogin.Password))
                    return true;
            }

            return false;
        }
    }


}

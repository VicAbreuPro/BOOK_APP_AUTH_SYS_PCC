using Users.Models;
using Users.Data;
using Microsoft.EntityFrameworkCore;

namespace Users.Repository
{
    public class UserRepository
    {
        private readonly DataContext _dataContext;

        public UserRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public int CheckUsernameAndEmail(string email, string username)
        {
            var checkUserName = _dataContext.Users.Where(user => user.UserName == username).FirstOrDefault();

            var checkEmail = _dataContext.Users.Where(user => user.Email == email).FirstOrDefault();

            if(checkUserName != null)
            {
                return 1;
            }

            if(checkEmail != null)
            {
                return 2;
            }

            return 0;
        }

        public int CreateUser(User user)
        {
            _dataContext.Users.Add(user);

            return _dataContext.SaveChanges();
        }

        public int CreateLoginAttempt(LoginAttempts attempt)
        {
            _dataContext.LoginAttempts.Add(attempt);

            return _dataContext.SaveChanges();
        }

        public int GetLoginAttempts(string ip)
        {
            return _dataContext.LoginAttempts.Where(la => la.email == ip).Count();
        }

        public LoginAttempts? GetLastLoginAttempt(string email)
        {
            return _dataContext.LoginAttempts.Where(la => la.email == email).OrderByDescending(la => la.CreatedAt).FirstOrDefault();
        }

        public int RemoveAllLoginAttempts(string ip)
        {
            return _dataContext.LoginAttempts.Where(la => la.email == ip).ExecuteDelete();
        }

        public User? GetUserByEmail(string email)
        {
            return _dataContext.Users.Where(user => user.Email == email).FirstOrDefault();
        }

        public User? GetUserById(int userId)
        {
            return _dataContext.Users.Find(userId);
        }

        public int UpdatePassword(User user, string password)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(password);

            return _dataContext.SaveChanges();
        }
    }
}

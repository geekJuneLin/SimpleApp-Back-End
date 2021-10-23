using SimpleApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleApp.Services
{
    public interface IUserRepository
    {
        public Task SaveUserToFile(User user);
        public Task<bool> IsUserExist(string email);
        public Task<User> GetUserByEmail(string email);
        public Task<ICollection<User>> GetUsers();

        public Task DeleteUser(User user);

        public Task SaveChanges();
    }
}

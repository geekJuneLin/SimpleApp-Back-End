using Newtonsoft.Json;
using SimpleApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleApp.Services
{
    public class UserRepository : IUserRepository
    {
        private string PATH_TO_SAVE = Environment.CurrentDirectory + "\\SavedJSONFiles\\saved.json";
        private ICollection<User> users = new List<User>(){};

        public async Task DeleteUser(User user)
        {
            users.Remove(user);
            await SaveChanges();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            // Get saved users first
            await GetSavedFileContent();

            return users.FirstOrDefault(user => user.Email == email);
        }

        public async Task<ICollection<User>> GetUsers()
        {
            await GetSavedFileContent();

            return users.ToArray();
        }

        public async Task<bool> IsUserExist(string email)
        {
            // Get saved users first
            await GetSavedFileContent();

            return users.Any(user => user.Email == email);
        }

        public async Task SaveChanges()
        {
            string jsonString = JsonConvert.SerializeObject(users);

            using (var textWriter = new StreamWriter(PATH_TO_SAVE))
            {
                await textWriter.WriteLineAsync(jsonString);
            }
        }

        public async Task SaveUserToFile(User user)
        {
            // Get saved users first
            await GetSavedFileContent();

            users.Add(user);
            string jsonString = JsonConvert.SerializeObject(users);

            using (var textWriter = new StreamWriter(PATH_TO_SAVE)) {
                await textWriter.WriteLineAsync(jsonString);
            }
        }

        private async Task GetSavedFileContent()
        {
            using (var sr = new StreamReader(PATH_TO_SAVE))
            {
                string savedResult = await sr.ReadToEndAsync();

                users.Clear();
                var savedUsers = JsonConvert.DeserializeObject<ICollection<User>>(savedResult);

                foreach (var user in savedUsers)
                {
                    users.Add(user);
                }
            }
        }


    }
}

using backend.Models;
using System.Collections.Generic;
using System.Linq;

namespace backend.Tests.TestData.Fakes
{
    public static class UserFactory
    {
        // A simple in-memory list of fake users
        private static readonly List<User> _fakeUsers = new()
        {
            new User { Id = 1, Name = "Alice", Email = "alice@example.com", PasswordHash = "hash1" },
            new User { Id = 2, Name = "Bob", Email = "bob@example.com", PasswordHash = "hash2" }
        };

        // Get all fake users
        public static List<User> GetFakeUsers() => _fakeUsers;

        // Get a single user by email
        public static User GetUserByEmail(string email) =>
            _fakeUsers.First(u => u.Email == email);

        // Get a single user by id
        public static User? GetUserById(int id) =>
            _fakeUsers.FirstOrDefault(u => u.Id == id);
    }
}

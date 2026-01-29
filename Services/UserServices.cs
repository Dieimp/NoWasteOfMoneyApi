using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using NoWasteOfMoney.Infrastructure.Database;
using NoWasteOfMoney.Interfaces;
using NoWasteOfMoney.Models.Entities;

namespace NoWasteOfMoney.Service.Services
{
    public class UserService : IUsersService
    {
        private readonly DatabaseContext _context;

        public UserService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<User?> Login(string email, string password)
        {

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);


            if (user == null)
            {
                return null;
            }


            if (!user.Active)
            {
                return null;
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            if (!isPasswordValid)
            {
                return null;
            }

            return user;
        }
    }
}
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using NoWasteOfMoney.Infrastructure.Database;
using NoWasteOfMoney.Interfaces;
using NoWasteOfMoney.Models.Entities;
using NoWasteOfMoney.Models.Entities.NoWasteOfMoney.Domain.Entities;

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

            Person person = new Person();
            person = await _context.Persons.FirstOrDefaultAsync(p => p.Email == email);
            if (person == null)
            {
                return null;
            }
            Console.WriteLine("passou person");

            var user = await _context.Users
                .Include(u => u.Person)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.PersonId == person.Id);
            Console.WriteLine(user.PasswordHash);

            // Console.WriteLine(BCrypt.Net.BCrypt.HashPassword(user.PasswordHash));

<<<<<<< Updated upstream
=======
            var user = await _context.Users
                .Include(u => u.Person)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.PersonId == person.Id);
            Console.WriteLine(user.PasswordHash);
>>>>>>> Stashed changes

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            Console.WriteLine(isPasswordValid);
            if (!isPasswordValid)
            {
                return null;
            }

            return user;
        }

        public async Task<User?> Create(User user)
        {

            Person person = new Person();

            person = await _context.Persons.FirstOrDefaultAsync(p => p.Id == user.PersonId);

            if (person == null)
            {
                Console.WriteLine("Entrou no null nao achou user id");
                return null;
            }
            Console.WriteLine("Passou valiudacao de person" + person.Id);
            string newPassword = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            user.PasswordHash = newPassword;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;

        }
    }
}
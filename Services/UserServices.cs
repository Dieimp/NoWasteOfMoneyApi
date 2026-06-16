using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using NoWasteOfMoney.Infrastructure.Database;
using NoWasteOfMoney.Interfaces;
using NoWasteOfMoney.Models.Dtos;
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
            Console.WriteLine($"UserService.Login called for: {email}");
            Person person = new Person();
            person = await _context.Persons.FirstOrDefaultAsync(p => p.Email == email);
            if (person == null)
            {
                return null;
            }

            var user = await _context.Users
                .Include(u => u.Person)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.PersonId == person.Id);

            if (user == null)
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

        public async Task<User?> Create(User user)
        {
            Person person = new Person();
            person = await _context.Persons.FirstOrDefaultAsync(p => p.Id == user.PersonId);

            if (person == null)
            {
                return null;
            }

            string newPassword = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            user.PasswordHash = newPassword;
            user.Id = Guid.NewGuid();
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<UserResponse> CreateAccount(CreateUser createUser)
        {
            if (!UserRoles.IsValid(createUser.Role))
            {
                throw new InvalidOperationException($"Invalid role: {createUser.Role}");
            }

            // Verificação explícita de email duplicado
            var person = await _context.Persons.FirstOrDefaultAsync(p => p.Email == createUser.Email);
            if (person != null)
            {
                return null;
            }

            // Gerar ID do Person antes de criar a entidade
            var newPersonId = Guid.NewGuid();
            //Gera senha temporaria 
            string temporaryPassword = $"NWM@{Guid.NewGuid().ToString()[..6]}";

            var newUser = new User
            {
                PersonId = newPersonId,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(temporaryPassword),
                Role = createUser.Role,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                PasswordResetToken = Guid.NewGuid().ToString(),
                ResetTokenExpiresAt = DateTime.UtcNow.AddHours(48)
            };

            var personEntity = new Person
            {
                Id = newPersonId,
                FirstName = createUser.Name,
                LastName = "",
                Email = createUser.Email,

            };

            _context.Persons.Add(personEntity);
            _context.Users.Add(newUser);

            await _context.SaveChangesAsync();

            return new UserResponse(
                Id: newUser.Id,
                PersonId: personEntity.Id,
                Name: personEntity.FirstName,
                Email: personEntity.Email,
                Role: newUser.Role,
                CreatedAt: newUser.CreatedAt,
                TemporaryPassword: temporaryPassword,
                ActivationToken: newUser.PasswordResetToken
            );
        }
    }
}

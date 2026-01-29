using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NoWasteOfMoney.Models.Entities;

namespace NoWasteOfMoney.Interfaces
{
    public interface IUsersService
    {
        Task<User?> Login(string email, string password);
    }
}
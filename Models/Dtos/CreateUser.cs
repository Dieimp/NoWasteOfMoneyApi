using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoWasteOfMoney.Models.Dtos
{
    public record CreateUser
    (
        int Id
       , int UserId
       , string PasswordHash
       , string Role
       , DateTime CreatedAt
    );
}
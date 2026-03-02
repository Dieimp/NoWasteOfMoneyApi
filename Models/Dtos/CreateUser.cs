using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NoWasteOfMoney.Models.Dtos
{
    public record CreateUser
    (
        [Required] Guid UserId
       , [Required] string PasswordHash
       , [Required] string Role
       , [Required] DateTime CreatedAt
    );
}
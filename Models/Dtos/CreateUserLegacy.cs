using System;
using System.ComponentModel.DataAnnotations;

namespace NoWasteOfMoney.Models.Dtos
{
    public record CreateUserLegacy
    (
        [Required] Guid UserId
       , [Required] string PasswordHash
       , [Required] string Role
       , [Required] DateTime CreatedAt
    );
}

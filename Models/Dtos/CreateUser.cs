using System;
using System.ComponentModel.DataAnnotations;

namespace NoWasteOfMoney.Models.Dtos
{
    public record CreateUser
    (
        [Required] string Name
       , [Required, EmailAddress] string Email
    //    , [Required] string Password
       , [Required] string Role
    );
}
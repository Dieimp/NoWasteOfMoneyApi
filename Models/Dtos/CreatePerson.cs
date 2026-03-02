using System;
using System.ComponentModel.DataAnnotations;

namespace NoWasteOfMoney.Models.Dtos
{
    public record CreatePerson
    (
        [Required, StringLength(100)] string FirstName,
        [StringLength(255)] string LastName,
        [Required, StringLength(255), EmailAddress] string Email
    );
}

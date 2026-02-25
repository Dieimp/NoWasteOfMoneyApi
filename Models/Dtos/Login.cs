using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NoWasteOfMoney.Models.Dtos
{
    public record Login(
        [Required][EmailAddress(ErrorMessage = "O email fornecido não é válido.")] string Email,
        [Required] string Password

    );
}
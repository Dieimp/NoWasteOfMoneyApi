using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoWasteOfMoney.Models.Dtos
{
    public record LoginResponseDto(
        string AccessToken,
        DateTime ExpiresAt,
        string Name,
        string Email,
        Guid PersonId
    // string Password
    );
}
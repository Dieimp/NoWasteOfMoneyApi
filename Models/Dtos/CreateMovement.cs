using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NoWasteOfMoney.Models.Dtos
{
    public record CreateMovementRequest(
        [Required][StringLength(255)] string Name,
        [Required][StringLength(500)] string Description,
        [Required] int MovementTypeId
    );
}
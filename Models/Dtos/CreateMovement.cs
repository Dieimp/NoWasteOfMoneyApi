using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NoWasteOfMoney.Models.Dtos
{
    public record CreateMovementRequest(
        [Required] string Name,
        [Required] string Description,
        [Required] int MovementTypeId
    );
}
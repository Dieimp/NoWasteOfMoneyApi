using System.Collections.Generic;
using NoWasteOfMoney.Models.Entities;

namespace NoWasteOfMoney.Models.Dtos
{
    public record MonthResumeDto(List<MonthMovement> Movements, decimal Total);
}

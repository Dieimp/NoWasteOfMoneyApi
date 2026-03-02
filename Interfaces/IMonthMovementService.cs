using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NoWasteOfMoney.Models.Dtos;
using NoWasteOfMoney.Models.Entities;

namespace NoWasteOfMoney.Interfaces
{
    public interface IMonthMovementService
    {
        Task<PagedResult<MonthMovement>> GetAll(int pageNumber, int pageSize, Guid? personId = null);
        Task<PagedResult<MonthMovement>> GetByMonth(int pageNumber, int pageSize, Guid personId, DateOnly date);
        Task<MonthResumeDto> GetMonthResume(Guid personId, DateOnly date);
        Task<MonthMovement> Create(Guid personId, MonthMovement monthMovement);
        Task<MonthMovement?> Update(Guid id, MonthMovement monthMovement);
        Task<bool> Delete(Guid id);
    }
}
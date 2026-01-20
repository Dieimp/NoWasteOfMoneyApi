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
        Task<PagedResult<MonthMovement>> GetAll(int pageNumber, int pageSize, int personId);
        Task<PagedResult<MonthMovement>> GetByMonth(int pageNumber, int pageSize, int personId, DateOnly date);
        Task<MonthMovement> Create(int personId, MonthMovement monthMovement);
        Task<MonthMovement?> Update(int id, MonthMovement monthMovement);
        Task<bool> Delete(int id);
    }
}
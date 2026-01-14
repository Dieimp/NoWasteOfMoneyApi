using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NoWasteOfMoney.Models.Dtos;
using NoWasteOfMoney.Models.Entities;

namespace NoWasteOfMoney.Interfaces
{
    public interface IMovementService
    {
        Task<PagedResult<Movement>> GetAll(int pageNumber, int pageSize);
        Task<Movement?> GetById(int id);
        Task<Movement> Create(Movement movement);
        Task<Movement?> Update(int id, Movement movement);
        Task<bool> Delete(int id);
    }
}
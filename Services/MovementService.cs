using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NoWasteOfMoney.Infrastructure.Database;
using NoWasteOfMoney.Interfaces;
using NoWasteOfMoney.Models.Dtos;
using NoWasteOfMoney.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace NoWasteOfMoney.Services
{
    public class MovementService : IMovementService
    {
        private readonly DatabaseContext _context;

        public MovementService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Movement>> GetAll(int pageNumber, int pageSize)
        {

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;


            int totalCount = await _context.Movements.CountAsync();

            int skipAmount = (pageNumber - 1) * pageSize;

            var movements =
                await _context.Movements
                    .OrderBy(p => p.Name)
                    .Skip(skipAmount)
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToListAsync();

            return new PagedResult<Movement>
            {
                Items = movements,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

        }

        public async Task<Movement?> GetById(int id)
        {
            return await _context.Movements.FindAsync(id);
        }

        public async Task<Movement> Create(Movement movement)
        {
            _context.Movements.Add(movement);
            await _context.SaveChangesAsync();
            return movement;
        }

        public async Task<Movement?> Update(int id, Movement movement)
        {
            var findedMovement = await _context.Movements.FindAsync(id);

            if (findedMovement == null)
            {
                return null;
            }

            findedMovement.Name = movement.Name;
            findedMovement.Description = movement.Description;
            findedMovement.MovementTypeId = movement.MovementTypeId;


            //TODO: ADICIONAR VALIDACAO SE O UPDATE REALMANTE ACONTECE
            await _context.SaveChangesAsync();
            return findedMovement;


        }

        // DELETE
        public async Task<bool> Delete(int id)
        {
            var findedMovement = await _context.Movements.FindAsync(id);
            if (findedMovement == null)
            {
                return false;
            }

            _context.Movements.Remove(findedMovement);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
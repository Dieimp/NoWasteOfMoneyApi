using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NoWasteOfMoney.Infrastructure.Database;
using NoWasteOfMoney.Interfaces;
using NoWasteOfMoney.Models.Dtos;
using NoWasteOfMoney.Models.Entities;

namespace NoWasteOfMoney.Services
{
    public class MonthMovementsService : IMonthMovementService
    {

        private readonly DatabaseContext _context;

        public MonthMovementsService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<MonthMovement> Create(int personId, MonthMovement monthMovement)
        {
            try
            {
                if (!await PersonExistsLegacy(personId, monthMovement))
                {
                    throw new ArgumentException("Houve um problema com a pessoa indicada,valide as informacoes e tente novamente");
                }
                if (!await MovementExists(monthMovement.MovementId))
                {
                    throw new ArgumentException("Houve um problema com a movimentacao indicada,valide as informacoes e tente novamente");
                }

                _context.MonthMovements.Add(monthMovement);
                await _context.SaveChangesAsync();

                return monthMovement;

            }
            catch (Exception ex)
            {
                throw new Exception("houve um erro " + ex.ToString());
            }
        }

        //duvida 
        public async Task<bool> Delete(int id)
        {
            var findedMonthMovement = await _context.MonthMovements.FindAsync(id);
            if (findedMonthMovement == null)
            {
                return false;
            }

            _context.MonthMovements.Remove(findedMonthMovement);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<PagedResult<MonthMovement>> GetAll(int pageNumber, int pageSize, int? personId = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _context.MonthMovements.AsQueryable();

            if (personId.HasValue)
            {
                if (!await PersonExists(personId.Value))
                {
                    throw new ArgumentException("Houve um problema com a pessoa indicada,valide as informacoes e tente novamente");
                }
                query = query.Where(m => m.PersonId == personId.Value);
            }

            int totalCount = await query.CountAsync();
            int skipAmount = (pageNumber - 1) * pageSize;

            var monthMovements =
                await query
                    .OrderBy(p => p.Year).ThenBy(p => p.Month)
                    .Skip(skipAmount)
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToListAsync();

            return new PagedResult<MonthMovement>
            {
                Items = monthMovements,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }


        public async Task<PagedResult<MonthMovement>> GetByMonth(int pageNumber, int pageSize, int personId, DateOnly referenceDate)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            int year = referenceDate.Year;
            int month = referenceDate.Month;

            if (!await PersonExists(personId))
            {
                throw new ArgumentException("Houve um problema com a pessoa indicada,valide as informacoes e tente novamente");
            }

            var query = _context.MonthMovements
                .Where(m => m.PersonId == personId && m.Year == year && m.Month == month);

            int totalCount = await query.CountAsync();
            int skipAmount = (pageNumber - 1) * pageSize;

            var monthMovements =
                await query
                    .Include(m => m.Movement)
                    .OrderBy(m => m.Year)
                    .ThenBy(m => m.Month)
                    .Skip(skipAmount)
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToListAsync();

            return new PagedResult<MonthMovement>
            {
                Items = monthMovements,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<MonthResumeDto> GetMonthResume(int personId, DateOnly referenceDate)
        {
            int year = referenceDate.Year;
            int month = referenceDate.Month;

            if (!await PersonExists(personId))
            {
                throw new ArgumentException("Houve um problema com a pessoa indicada,valide as informacoes e tente novamente");
            }

            var movements = await _context.MonthMovements
                .Include(m => m.Movement)
                .ThenInclude(mv => mv.MovementType)
                .Where(m => m.PersonId == personId && m.Year == year && m.Month == month)
                .OrderBy(m => m.Year)
                .ThenBy(m => m.Month)
                .AsNoTracking()
                .ToListAsync();

            decimal total = 0;
            foreach (var mov in movements)
            {
                if (mov.Movement != null)
                {
                    if (mov.Movement.MovementTypeId == 1) // Debit
                    {
                        total -= mov.Value;
                    }
                    else if (mov.Movement.MovementTypeId == 2) // Credit
                    {
                        total += mov.Value;
                    }
                }
            }

            return new MonthResumeDto(movements, total);
        }

        public async Task<MonthMovement?> Update(int id, MonthMovement monthMovement)
        {
            var findedMonthMovement = await _context.MonthMovements.FindAsync(id);

            if (findedMonthMovement == null)
            {
                return null;
            }

            if (!await PersonExistsLegacy(findedMonthMovement.PersonId, monthMovement))
            {
                throw new ArgumentException("Houve um problema com a pessoa indicada,valide as informacoes e tente novamente");
            }
            if (!await MovementExists(monthMovement.MovementId))
            {
                throw new ArgumentException("Houve um problema com a movimentacao indicada,valide as informacoes e tente novamente");
            }

            findedMonthMovement.MovementId = monthMovement.MovementId;
            //pensar se faz sentido
            findedMonthMovement.PersonId = monthMovement.PersonId;
            findedMonthMovement.Year = monthMovement.Year;
            findedMonthMovement.Value = monthMovement.Value;

            //TODO: ADICIONAR VALIDACAO SE O UPDATE REALMANTE ACONTECE
            await _context.SaveChangesAsync();
            return findedMonthMovement;
        }

        private async Task<bool> PersonExists(int personId)
        {
            return await _context.Persons.AnyAsync(p => p.Id == personId);
        }

        private async Task<bool> PersonExistsLegacy(int personId, MonthMovement monthMovement)
        {
            return personId == monthMovement.PersonId &&
               await _context.Persons.AnyAsync(p => p.Id == personId);
        }

        private async Task<bool> MovementExists(int movementId)
        {
            return await _context.Movements.AnyAsync(m => m.Id == movementId);
        }
    }
}
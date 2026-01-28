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
                if (!await PersonExists(personId, monthMovement))
                {
                    throw new ArgumentException("Houve um problema com a pessoa indicada,valide as informacoes e tente novamente");
                }
                if (!await MovementExists(monthMovement))
                {
                    throw new ArgumentException("Houve um problema com a movimentacao indicada,valide as informacoes e tente novamente");
                }

                _context.MonthMovements.Add(monthMovement);
                await _context.SaveChangesAsync();

                return monthMovement;

            }
            catch (Exception ex)
            {
                throw new Exception("houve um erro" + ex.ToString());
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

        public async Task<PagedResult<MonthMovement>> GetAll(int pageNumber, int pageSize, int personId)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;



            int totalCount = await _context.MonthMovements.CountAsync(m => m.PersonId == personId); ;

            if (totalCount == 0)
            {
                throw new ArgumentException("Houve um problema com a pessoa indicada,valide as informacoes e tente novamente");
            }

            int skipAmount = (pageNumber - 1) * pageSize;

            var monthMovements =
                await _context.MonthMovements
                    .OrderBy(p => new { p.Year, p.Month })
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

            int Year;
            int month;
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            Year = referenceDate.Year;
            month = referenceDate.Month;

            int totalCount = await _context.MonthMovements.CountAsync(m => m.PersonId == personId); ;

            if (totalCount == 0)
            {
                throw new ArgumentException("Houve um problema com a pessoa indicada,valide as informacoes e tente novamente");
            }

            int skipAmount = (pageNumber - 1) * pageSize;

            var monthMovements =
                await _context.MonthMovements
                    .Include(m => m.Movement)
                    .Where(m => m.Year == Year && m.Month == month)
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

        public async Task<MonthMovement?> Update(int id, MonthMovement monthMovement)
        {
            var findedMonthMovement = await _context.MonthMovements.FindAsync(id);

            if (findedMonthMovement == null)
            {
                return null;
            }

            if (!await PersonExists(findedMonthMovement.PersonId, monthMovement))
            {
                throw new ArgumentException("Houve um problema com a pessoa indicada,valide as informacoes e tente novamente");
            }
            if (!await MovementExists(monthMovement))
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

        private async Task<bool> PersonExists(int personId, MonthMovement monthMovement)
        {

            return personId == monthMovement.PersonId &&
               await _context.Persons.AnyAsync(p => p.Id == personId);
        }

        private async Task<bool> MovementExists(MonthMovement monthMovement)
        {
            MonthMovement movement;

            movement = await _context.MonthMovements.FindAsync(monthMovement.MovementId);

            if (movement == null)
            {
                return true;
            }
            else
            {
                return false;
            }


        }
    }
}
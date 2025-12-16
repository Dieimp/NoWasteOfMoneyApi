using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NoWasteOfMoney.Infrastructure.Database;
using NoWasteOfMoney.Interfaces;
using NoWasteOfMoney.Models.Entities;
using NoWasteOfMoney.Models.Dtos;

namespace NoWasteOfMoney.Services
{
    public class PersonsService : IPersonsService
    {
        private readonly DatabaseContext _context;

        public PersonsService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Person>> GetAll(int pageNumber, int pageSize)
        {

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;


            int totalCount = await _context.Persons.CountAsync();

            int skipAmount = (pageNumber - 1) * pageSize;

            var persons =
                await _context.Persons
                    .OrderBy(p => p.FirstName)
                    .Skip(skipAmount)
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToListAsync();

            return new PagedResult<Person>
            {
                Items = persons,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

        }

        public async Task<Person?> GetById(int id)
        {
            return await _context.Persons.FindAsync(id);
        }

        public async Task<Person> Create(Person person)
        {
            _context.Persons.Add(person);
            await _context.SaveChangesAsync();
            return person;
        }

        public async Task<Person?> Update(int id, Person person)
        {
            var findedPerson = await _context.Persons.FindAsync(id);

            if (findedPerson == null)
            {
                return null;
            }

            findedPerson.FirstName = person.FirstName;
            findedPerson.LastName = person.LastName;
            findedPerson.Email = person.Email;

            //TODO: ADICIONAR VALIDACAO SE O UPDATE REALMANTE ACONTECE
            await _context.SaveChangesAsync();
            return findedPerson;


        }

        // DELETE
        public async Task<bool> Delete(int id)
        {
            var findedPerson = await _context.Persons.FindAsync(id);
            if (findedPerson == null)
            {
                return false;
            }

            _context.Persons.Remove(findedPerson);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
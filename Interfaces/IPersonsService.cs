using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NoWasteOfMoney.Models.Entities;
using NoWasteOfMoney.Models.Dtos;

namespace NoWasteOfMoney.Interfaces
{
    public interface IPersonsService
    {
        Task<PagedResult<Person>> GetAll(int pageNumber, int pageSize);

        //Task<IEnumerable<Person>> GetAll(int page, int lenght);
        Task<Person?> GetById(int id);
        Task<Person> Create(Person person);
        Task<Person?> Update(int id, Person person);
        Task<bool> Delete(int id);

    }
}
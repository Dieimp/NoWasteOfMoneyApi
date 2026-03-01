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
        Task<Person?> GetById(Guid id);
        Task<Person> Create(Person person);
        Task<Person?> Update(Guid id, Person person);
        Task<bool> Delete(Guid id);

    }
}
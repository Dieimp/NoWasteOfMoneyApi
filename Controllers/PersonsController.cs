using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NoWasteOfMoney.Interfaces;
using NoWasteOfMoney.Models.Entities;
using NoWasteOfMoney.Models.Dtos;
using NoWasteOfMoney.Services;


namespace NoWasteOfMoney.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonsController : ControllerBase
    {
        private readonly IPersonsService _service;

        public PersonsController(IPersonsService service)
        {
            _service = service;
        }

        // GET: api/Persons?pageNumber=2&pageSize=10
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<Person>>> GetPessoas(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var pagedResult = await _service.GetAll(pageNumber, pageSize);

            return Ok(pagedResult);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Person>> ReadById(Guid id)
        {
            var person = await _service.GetById(id);

            if (person == null)
            {
                return NotFound($"Pessoa com ID {id} não encontrada.");
            }

            return Ok(person);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Person>> Create(CreatePerson createPerson)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var person = new Person
            {
                FirstName = createPerson.FirstName,
                LastName = createPerson.LastName,
                Email = createPerson.Email
            };

            var novaPessoa = await _service.Create(person);
            return CreatedAtAction(nameof(Create), new { id = novaPessoa.Id }, novaPessoa);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, Person person)
        {
            if (id != person.Id)
            {
                return BadRequest("O ID na URL deve corresponder ao ID no corpo da requisição.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedPerson = await _service.Update(id, person);

            if (updatedPerson == null)
            {
                return NotFound($"Pessoa com ID {id} não encontrada.");
            }
            return Ok(updatedPerson);

        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var isDeleted = await _service.Delete(id);

            if (!isDeleted)
            {
                return NotFound($"Pessoa com ID {id} não encontrada.");
            }

            return NoContent();
        }
    }
}
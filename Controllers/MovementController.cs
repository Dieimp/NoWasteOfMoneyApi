using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NoWasteOfMoney.Interfaces;
using NoWasteOfMoney.Models.Dtos;
using NoWasteOfMoney.Models.Entities;

namespace NoWasteOfMoney.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovementController : ControllerBase
    {
        private readonly IMovementService _service;

        public MovementController(IMovementService service)
        {
            _service = service;
        }

        // GET: api/Pessoas?pageNumber=2&pageSize=10
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<Movement>>> GetMovements(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var pagedResult = await _service.GetAll(pageNumber, pageSize);

            return Ok(pagedResult);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Movement>> ReadById(int id)
        {
            var movement = await _service.GetById(id);

            if (movement == null)
            {
                return NotFound($"Pessoa com ID {id} não encontrada.");
            }

            return Ok(movement);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Movement>> Create(CreateMovementRequest req)
        {

            var movement = new Movement
            {
                Name = req.Name,
                Description = req.Description,
                MovementTypeId = req.MovementTypeId
            };

            try 
            {
                await _service.Create(movement);
                return Ok(movement.Id);
            } 
            catch (ArgumentException ex) 
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, CreateMovementRequest req)
        {
            var movement = new Movement
            {
                Name = req.Name,
                Description = req.Description,
                MovementTypeId = req.MovementTypeId
            };

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try 
            {
                var updatedMovement = await _service.Update(id, movement);

                if (updatedMovement == null)
                {
                    return NotFound($"Pessoa com ID {id} não encontrada.");
                }
                return Ok(updatedMovement);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
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
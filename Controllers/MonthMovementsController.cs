using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NoWasteOfMoney.Interfaces;
using NoWasteOfMoney.Models.Dtos;
using NoWasteOfMoney.Models.Entities;

namespace NoWasteOfMoney.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MonthMovementsController : ControllerBase
    {
        private readonly IMonthMovementService _service;

        public MonthMovementsController(IMonthMovementService service)
        {
            _service = service;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<MonthMovement>>> GetMonthMovements(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int personId = 10)
        {
            var pagedResult = await _service.GetAll(pageNumber, pageSize, personId);
            return Ok(pagedResult);
        }
        [HttpGet("{personId},{date}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<PagedResult<MonthMovement>>> GetMonthMovementsByMonth(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int personId = 0,
            [FromQuery] DateOnly? date = null
            )

        {
            DateOnly refDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow);
            var pagedResult = await _service.GetByMonth(pageNumber, pageSize, personId, refDate);
            if (pagedResult == null)
            {
                return NotFound($"Pessoa com ID {personId} não encontrada.");

            }
            return Ok(pagedResult);
        }

        [HttpPost("{personId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MonthMovement>> Create(int personId, CreateMonthMovement req)
        {

            var monthMovement = new MonthMovement
            {
                MovementId = req.MovementId,
                PersonId = req.PersonId,
                Year = req.Date.Year,
                Month = req.Date.Month,
                Value = req.Value,

            };

            await _service.Create(personId, monthMovement);

            return Ok(monthMovement.Id);
        }


    }
}
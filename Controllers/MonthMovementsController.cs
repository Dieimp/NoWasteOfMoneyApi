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
            [FromQuery] Guid? personId = null
        )
        {
            var pagedResult = await _service.GetAll(pageNumber, pageSize, personId);
            return Ok(pagedResult);
        }

        [HttpGet("person/{personId:guid}/date/{date}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<PagedResult<MonthMovement>>> GetMonthMovementsByMonth(
                    Guid personId,
                    DateOnly? date,
                    [FromQuery] int pageNumber = 1,
                    [FromQuery] int pageSize = 10
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

        [HttpGet("person/{personId:guid}/resume/{date}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MonthResumeDto>> GetMonthResume(
            Guid personId,
            DateOnly? date
        )
        {
            try 
            {
                DateOnly refDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow);
                var resume = await _service.GetMonthResume(personId, refDate);
                return Ok(resume);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{personId:guid}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MonthMovement>> Create(Guid personId, CreateMonthMovement req)
        {

            var monthMovement = new MonthMovement
            {
                MovementId = req.MovementId,
                Year       = req.Date.Year,
                Month      = req.Date.Month,
                Value      = req.Value,
            };
            monthMovement.PersonId = personId;
            await _service.Create(personId, monthMovement);

            return Ok(monthMovement.Id);
        }


    }
}
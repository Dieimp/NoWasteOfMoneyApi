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
        public async Task<ActionResult<PagedResult<MonthMovement>>> GetMonthMovements(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int personId = 10)
        {
            var pagedResult = await _service.GetAll(pageNumber, pageSize, personId);
            return Ok(pagedResult);
        }


    }
}
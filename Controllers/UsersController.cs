using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoWasteOfMoney.Interfaces;
using NoWasteOfMoney.Models.Dtos;
using NoWasteOfMoney.Models.Entities;

namespace NoWasteOfMoney.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _service;
        private readonly TokenService _tokenService;

        public UsersController(IUsersService service, TokenService tokenService)
        {
            _service = service;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] Login login)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _service.Login(login.Email, login.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "E-mail ou senha inválidos." });
            }

            var (token, expiresAt) = _tokenService.GenerateToken(user);

            return Ok(new LoginResponseDto(
                AccessToken: token,
                ExpiresAt: expiresAt,
                Name: user.Person.FirstName,
                Email: user.Person.Email,
                PersonId: user.PersonId
            ));
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUser createUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!UserRoles.IsValid(createUser.Role))
            {
                return BadRequest(new { message = $"Invalid role: {createUser.Role}" });
            }

            var createdUser = await _service.CreateAccount(createUser);
            if (createdUser == null)
            {
                return Conflict(new { message = "E-mail já cadastrado." });
            }

            return CreatedAtAction(nameof(Create), new { id = createdUser.Id }, createdUser);
        }
    }
}

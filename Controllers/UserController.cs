
using Microsoft.AspNetCore.Mvc;

using NoWasteOfMoney.Interfaces;
using NoWasteOfMoney.Models.Dtos;
using NoWasteOfMoney.Models.Entities;
using NoWasteOfMoney.Models.Entities.NoWasteOfMoney.Domain.Entities;


namespace NoWasteOfMoney.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUsersService _service;

        private readonly TokenService _tokenService;
        public UserController(IUsersService service, TokenService tokenService)
        {
            _service = service;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
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
                Email: user.Person.Email
            ));
        }
        [HttpPost("create")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult<User>> Create(CreateUser createUser)
        {
            Console.WriteLine("entrou na funcao");

            var user = new User
            {
                Id = createUser.Id
                ,
                PersonId = createUser.UserId
                ,
                PasswordHash = createUser.PasswordHash
                ,
                Role = createUser.Role
                ,
                CreatedAt = createUser.CreatedAt
                ,
                UpdatedAt = null
            };

            Console.WriteLine(user.PersonId);
            var newUser = await _service.Create(user);
            return CreatedAtAction(nameof(Create), new { id = newUser.Id }, newUser);
        }


    }
}
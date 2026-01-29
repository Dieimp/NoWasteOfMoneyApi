
using Microsoft.AspNetCore.Mvc;

using NoWasteOfMoney.Interfaces;
using NoWasteOfMoney.Models.Dtos;
using NoWasteOfMoney.Models.Entities;


namespace NoWasteOfMoney.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUsersService _service;

        private readonly TokenService _tokenService;
        public UserController(IUsersService service)
        {
            _service = service;
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
                Name: user.Name,
                Email: user.Email
            ));
        }


    }
}
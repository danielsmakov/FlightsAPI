using FlightsAPI.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlightsAPI.Controllers
{
    /// <summary>
    /// Контроллер для аутентификации пользователей.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Аутентификация пользователя.
        /// </summary>
        /// <param name="command">Команда с данными для аутентификации пользователя (логин и пароль).</param>
        /// <returns>Возвращает JWT-токен при успешной аутентификации.</returns>
        /// <response code="200">Аутентификация успешна. Возвращён токен.</response>
        /// <response code="400">Неверные данные для аутентификации.</response>
        /// <response code="401">Пользователь не авторизован (например, неверный логин или пароль).</response>
        [HttpPost("login")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] AuthenticateUserCommand command)
        {
            var token = await _mediator.Send(command);
            return Ok(new { Token = token });
        }
    }
}

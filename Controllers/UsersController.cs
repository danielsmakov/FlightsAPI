using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlightsAPI.Controllers
{
    /// <summary>
    /// Контроллер для управления пользователями.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Регистрация нового пользователя (в целях демонстрации).
        /// </summary>
        /// <param name="username">Имя пользователя для регистрации.</param>
        /// <param name="password">Пароль для пользователя.</param>
        /// <param name="role">Роль, которая будет назначена пользователю.</param>
        /// <returns>Сообщение об успешной регистрации пользователя или ошибка.</returns>
        /// <response code="200">Пользователь успешно зарегистрирован.</response>
        /// <response code="400">Ошибка валидации или другая ошибка при создании пользователя.</response>
        [HttpPost("register")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<IdentityError>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(string username, string password, string role)
        {
            var user = new IdentityUser { UserName = username };
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }

            await _userManager.AddToRoleAsync(user, role);
            return Ok("Пользователь успешно зарегистрирован.");
        }
    }
}

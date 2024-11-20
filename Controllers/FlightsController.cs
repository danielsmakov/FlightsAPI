using FlightsAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightsAPI.Models.Entities;
using FlightsAPI.Queries;
using FlightsAPI.Handlers;
using MediatR;
using FlightsAPI.Commands;
using Microsoft.AspNetCore.Authorization;
using FlightsAPI.Models.DTOs;
using FlightsAPI.Models;
using Serilog;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace FlightsAPI.Controllers
{
    /// <summary>
    /// Контроллер для управления рейсами.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FlightController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FlightController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получает список рейсов на основе указанных фильтров.
        /// </summary>
        /// <param name="query">Параметры запроса для фильтрации рейсов.</param>
        /// <returns>Возвращает список рейсов, соответствующих критериям.</returns>
        /// <response code="200">Список рейсов успешно получен.</response>
        /// <response code="400">Указаны некорректные параметры запроса.</response>
        [HttpGet]
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<FlightDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFlights([FromQuery] GetFlightsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Создаёт новый рейс.
        /// </summary>
        /// <param name="command">Данные рейса для создания.</param>
        /// <returns>Возвращает ID созданного рейса.</returns>
        /// <response code="200">Рейс успешно создан.</response>
        /// <response code="400">Некорректные данные для создания рейса.</response>
        /// <response code="401">Пользователь не авторизован.</response>
        /// <response code="403">Доступ запрещён для пользователей без роли "Moderator".</response>
        [HttpPost]
        [Authorize(Roles = "Moderator")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateFlight([FromBody] CreateFlightCommand command)
        {
            Log.Information("Пользователь {User} добавляет новый рейс {Flight}.", User.Identity.Name, command);
            var flightId = await _mediator.Send(command);
            Log.Information("Рейс с ID {FlightId} успешно добавлен пользователем {User}.", flightId, User.Identity.Name);
            return Ok(new { Id = flightId });
        }

        /// <summary>
        /// Обновляет статус существующего рейса.
        /// </summary>
        /// <param name="command">ID рейса и новый статус.</param>
        /// <returns>Возвращает пустой ответ, если обновление выполнено успешно.</returns>
        /// <response code="204">Рейс успешно обновлён.</response>
        /// <response code="404">Рейс с указанным ID не найден.</response>
        /// <response code="401">Пользователь не авторизован.</response>
        /// <response code="403">Доступ запрещён для пользователей без роли "Moderator".</response>
        [HttpPut]
        [Authorize(Roles = "Moderator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateFlight([FromBody] UpdateFlightCommand command)
        {
            Log.Information("Пользователь {User} обновляет статус рейса {FlightId} на {Status}.", User.Identity.Name, command.FlightId, command.Status);
            var success = await _mediator.Send(command);
            if (success)
            {
                Log.Information("Рейс {FlightId} успешно обновлён пользователем {User}.", command.FlightId, User.Identity.Name);
                return NoContent();
            }
            else
            {
                Log.Warning("Рейс {FlightId} не найден. Обновление пользователем {User} не выполнено.", command.FlightId, User.Identity.Name);
                return NotFound();
            }
        }
    }
}

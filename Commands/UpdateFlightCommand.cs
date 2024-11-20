using MediatR;
using FlightsAPI.Enums;

namespace FlightsAPI.Commands
{
    public class UpdateFlightCommand : IRequest<bool>
    {
        public int FlightId { get; set; }
        public Status Status { get; set; }
    }
}

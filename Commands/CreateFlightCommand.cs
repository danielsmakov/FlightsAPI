using MediatR;
using FlightsAPI.Enums;

namespace FlightsAPI.Commands
{
    public class CreateFlightCommand : IRequest<int>
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTimeOffset Departure { get; set; }
        public DateTimeOffset Arrival { get; set; }
        public Status Status { get; set; }
    }
}

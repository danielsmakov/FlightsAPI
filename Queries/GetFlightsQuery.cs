using MediatR;
using FlightsAPI.Models.DTOs;

namespace FlightsAPI.Queries
{
    public class GetFlightsQuery : IRequest<IEnumerable<FlightDTO>>
    {
        public string? Origin { get; set; }
        public string? Destination { get; set; }
    }
}
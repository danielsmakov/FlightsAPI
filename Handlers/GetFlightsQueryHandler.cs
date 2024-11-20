using FlightsAPI.Interfaces;
using FlightsAPI.Models.DTOs;
using FlightsAPI.Models.Entities;
using FlightsAPI.Queries;
using FlightsAPI.Repositories;
using MediatR;

namespace FlightsAPI.Handlers
{
    public class GetFlightsQueryHandler : IRequestHandler<GetFlightsQuery, IEnumerable<FlightDTO>>
    {
        private readonly IRepository<Flight> _repository;

        public GetFlightsQueryHandler(IRepository<Flight> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<FlightDTO>> Handle(GetFlightsQuery request, CancellationToken cancellationToken)
        {
            var flights = await _repository.GetAllAsync(query =>
            {
                if (!string.IsNullOrEmpty(request.Origin))
                {
                    query = query.Where(f => f.Origin == request.Origin);
                }

                if (!string.IsNullOrEmpty(request.Destination))
                {
                    query = query.Where(f => f.Destination == request.Destination);
                }

                return query;
            });

            return flights.Select(f => new FlightDTO
            {
                Id = f.Id,
                Origin = f.Origin,
                Destination = f.Destination,
                Status = f.Status,
                Arrival = f.Arrival,
                Departure = f.Departure
            }).ToList();
        }
    }
}

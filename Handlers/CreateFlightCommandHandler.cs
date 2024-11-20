using FlightsAPI.Commands;
using FlightsAPI.Interfaces;
using FlightsAPI.Models.Entities;
using MediatR;

namespace FlightsAPI.Handlers
{
    public class CreateFlightCommandHandler : IRequestHandler<CreateFlightCommand, int>
    {
        private readonly IRepository<Flight> _repository;

        public CreateFlightCommandHandler(IRepository<Flight> repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(CreateFlightCommand request, CancellationToken cancellationToken)
        {
            var flight = new Flight
            {
                Origin = request.Origin,
                Destination = request.Destination,
                Departure = request.Departure,
                Arrival = request.Arrival,
                Status = request.Status
            };

            await _repository.AddAsync(flight);
            return flight.Id;
        }
    }
}

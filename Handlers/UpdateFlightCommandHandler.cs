using FlightsAPI.Commands;
using FlightsAPI.Interfaces;
using FlightsAPI.Models.Entities;
using MediatR;

namespace FlightsAPI.Handlers
{
    public class UpdateFlightCommandHandler : IRequestHandler<UpdateFlightCommand, bool>
    {
        private readonly IRepository<Flight> _repository;

        public UpdateFlightCommandHandler(IRepository<Flight> repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateFlightCommand request, CancellationToken cancellationToken)
        {
            var flight = await _repository.GetByIdAsync(request.FlightId);
            if (flight == null) return false;

            flight.Status = request.Status;
            await _repository.UpdateAsync(flight);

            return true;
        }
    }
}

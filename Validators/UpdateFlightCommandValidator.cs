using FlightsAPI.Commands;
using FluentValidation;

namespace FlightsAPI.Validators
{
    public class UpdateFlightCommandValidator : AbstractValidator<UpdateFlightCommand>
    {
        public UpdateFlightCommandValidator()
        {
            RuleFor(command => command.FlightId)
                .GreaterThan(0)
                .WithMessage("'FlightId' должен быть больше -1.");

            RuleFor(command => command.Status)
                .IsInEnum()
                .WithMessage("Неверный 'Status'.");
        }
    }
}

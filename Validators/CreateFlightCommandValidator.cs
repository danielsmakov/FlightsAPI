using FlightsAPI.Commands;
using FluentValidation;

namespace FlightsAPI.Validators
{
    public class CreateFlightCommandValidator : AbstractValidator<CreateFlightCommand>
    {
        public CreateFlightCommandValidator()
        {
            RuleFor(command => command.Origin)
                .NotEmpty()
                .WithMessage("'Origin' обязателен к заполнению.")
                .MaximumLength(100)
                .WithMessage("'Origin' не должен быть длиннее 100 символов.");

            RuleFor(command => command.Destination)
                .NotEmpty()
                .WithMessage("'Destination' обязателен к заполнению.")
                .MaximumLength(100)
                .WithMessage("'Destination' не должен быть длиннее 100 символов.");

            RuleFor(command => command.Departure)
                .NotEmpty()
                .WithMessage("'Departure' обязателен к заполнению.")
                .Must(date => date > DateTime.UtcNow)
                .WithMessage("'Departure' должен быть в будущем.");

            RuleFor(command => command.Arrival)
                .NotEmpty()
                .WithMessage("'Arrival' обязателен к заполнению.")
                .GreaterThan(command => command.Departure)
                .WithMessage("'Arrival' должен быть позже 'Departure'.");

            RuleFor(command => command.Status)
                .IsInEnum()
                .WithMessage("Неверный 'Status'.");
        }
    }
}

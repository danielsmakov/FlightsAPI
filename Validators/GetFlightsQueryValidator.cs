using FlightsAPI.Queries;
using FluentValidation;

namespace FlightsAPI.Validators
{
    public class GetFlightsQueryValidator : AbstractValidator<GetFlightsQuery>
    {
        public GetFlightsQueryValidator()
        {
            RuleFor(query => query.Origin)
                .NotEmpty().When(query => string.IsNullOrEmpty(query.Destination))
                .WithMessage("Либо 'origin', либо 'destination', либо оба сразу должны быть заполнены.");

            RuleFor(query => query.Destination)
                .NotEmpty().When(query => string.IsNullOrEmpty(query.Origin))
                .WithMessage("Либо 'origin', либо 'destination', либо оба сразу должны быть заполнены.");
        }
    }
}

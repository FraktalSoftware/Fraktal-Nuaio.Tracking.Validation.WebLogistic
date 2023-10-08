using FluentValidation;
using Fraktal_Nuaio.Tracking.Validation.WebLogistic.Shared.Dtos;

namespace Fraktal_Nuaio.Tracking.Validation.WebLogistic.Shared.Validators
{
    public class WebLogisticEventValidator : AbstractValidator<WebLogisticEventDto>
    {
        public WebLogisticEventValidator()
        {
            RuleFor(x => x.EventType)
                .NotEmpty()
                .WithMessage("{PropertyName} can't be null or empty");
            RuleFor(x => x.Order)
                .NotEmpty()
                .WithMessage("{PropertyName} can't be 0");
            RuleFor(x => x.ReturnReasonName)
                .NotEmpty()
                .WithMessage("{PropertyName} can't be null or empty")
                .MaximumLength(100)
                .WithMessage("{PropertyName} can't be greater 100");
            RuleFor(x => x.StatusCode)
                .NotEmpty()
                .WithMessage("{PropertyName} can't be 0");
            RuleFor(x => x.StatusDate)
                .NotEmpty()
                .WithMessage("{PropertyName} can't be null or empty");
            RuleFor(x => x.StatusName)
                .NotEmpty()
                .WithMessage("{PropertyName} can't be null or empty")
                .MaximumLength(100)
                .WithMessage("{PropertyName} can't be greater 100");
            RuleFor(x => x.Ucid)
                .NotEmpty()
                .WithMessage("{PropertyName} can't be null or empty")
                .MaximumLength(100)
                .WithMessage("{PropertyName} can't be greater 100");
        }
    }
}

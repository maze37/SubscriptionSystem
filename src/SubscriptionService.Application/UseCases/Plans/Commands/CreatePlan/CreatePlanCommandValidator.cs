using FluentValidation;

namespace SubscriptionService.Application.UseCases.Plans.Commands.CreatePlan;

public class CreatePlanCommandValidator : AbstractValidator<CreatePlanCommand>
{
    public CreatePlanCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID плана не может быть пустым.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название плана не может быть пустым.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Цена должна быть больше нуля.");
    }
}
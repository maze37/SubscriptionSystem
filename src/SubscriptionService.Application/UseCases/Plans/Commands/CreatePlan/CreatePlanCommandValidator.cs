using FluentValidation;

namespace SubscriptionService.Application.UseCases.Plans.Commands.CreatePlan;

/// <summary>
/// Валидатор команды CreatePlanCommand.
/// Запускается автоматически через ValidationBehavior до хендлера.
/// </summary>
public class CreatePlanCommandValidator : AbstractValidator<CreatePlanCommand>
{
    public CreatePlanCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название плана не может быть пустым.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Цена должна быть больше нуля.");

        RuleFor(x => x.BillingPeriod)
            .IsInEnum().WithMessage("Некорректный billing period.");
    }
}

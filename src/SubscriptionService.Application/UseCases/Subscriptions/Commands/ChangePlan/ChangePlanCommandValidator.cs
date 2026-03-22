using FluentValidation;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.ChangePlan;

/// <summary>
/// Валидатор команды ChangePlanCommand.
/// Запускается автоматически через ValidationBehavior до хендлера.
/// </summary>
public class ChangePlanCommandValidator : AbstractValidator<ChangePlanCommand>
{
    public ChangePlanCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .NotEmpty().WithMessage("ID подписки не может быть пустым.");

        RuleFor(x => x.NewPlanId)
            .NotEmpty().WithMessage("ID нового плана не может быть пустым.");
    }
}
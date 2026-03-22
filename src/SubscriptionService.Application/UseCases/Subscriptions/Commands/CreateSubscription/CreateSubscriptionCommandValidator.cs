using FluentValidation;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.CreateSubscription;

/// <summary>
/// Валидатор команды CreateSubscriptionCommand.
/// Запускается автоматически через ValidationBehavior до хендлера.
/// </summary>
public class CreateSubscriptionCommandValidator : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("ID пользователя не может быть пустым.");

        RuleFor(x => x.PlanId)
            .NotEmpty().WithMessage("ID плана не может быть пустым.");
    }
}
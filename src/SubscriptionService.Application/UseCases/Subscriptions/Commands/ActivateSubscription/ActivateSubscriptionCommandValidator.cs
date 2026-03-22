using FluentValidation;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.ActivateSubscription;

/// <summary>
/// Валидатор команды ActivateSubscriptionCommand.
/// Запускается автоматически через ValidationBehavior до хендлера.
/// </summary>
public class ActivateSubscriptionCommandValidator : AbstractValidator<ActivateSubscriptionCommand>
{
    public ActivateSubscriptionCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .NotEmpty().WithMessage("ID подписки не может быть пустым.");

        RuleFor(x => x.InvoiceId)
            .NotEmpty().WithMessage("ID счёта не может быть пустым.");
    }
}
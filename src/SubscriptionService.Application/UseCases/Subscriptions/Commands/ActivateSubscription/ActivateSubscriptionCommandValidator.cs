using FluentValidation;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.ActivateSubscription;

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
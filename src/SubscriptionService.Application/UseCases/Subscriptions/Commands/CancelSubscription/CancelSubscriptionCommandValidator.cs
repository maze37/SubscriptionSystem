using FluentValidation;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.CancelSubscription;

public class CancelSubscriptionCommandValidator : AbstractValidator<CancelSubscriptionCommand>
{
    public CancelSubscriptionCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .NotEmpty().WithMessage("ID подписки не может быть пустым.");
    }
}
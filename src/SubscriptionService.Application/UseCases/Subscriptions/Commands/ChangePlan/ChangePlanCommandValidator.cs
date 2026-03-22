using FluentValidation;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.ChangePlan;

public class ChangePlanCommandValidator : AbstractValidator<ChangePlanCommand>
{
    public ChangePlanCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .NotEmpty().WithMessage("ID подписки не может быть пустым.");

        RuleFor(x => x.NewPlanId)
            .NotEmpty().WithMessage("ID нового плана не может быть пустым.");

        RuleFor(x => x.InvoiceId)
            .NotEmpty().WithMessage("ID счёта не может быть пустым.");
    }
}
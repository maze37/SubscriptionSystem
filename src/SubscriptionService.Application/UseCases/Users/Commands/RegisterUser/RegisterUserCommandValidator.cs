using FluentValidation;

namespace SubscriptionService.Application.UseCases.Users.Commands.RegisterUser;

/// <summary>
/// Валидатор команды RegisterUserCommand.
/// Запускается автоматически через ValidationBehavior до хендлера.
/// </summary>
public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email не может быть пустым.")
            .EmailAddress().WithMessage("Некорректный формат email.");
    }
}
using FluentValidation;

namespace SubscriptionService.Application.UseCases.Users.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID пользователя не может быть пустым.");
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email не может быть пустым.")
            .EmailAddress().WithMessage("Некорректный формат email.");
    }
}
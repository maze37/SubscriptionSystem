using SubscriptionService.Application.Abstractions.Core;

namespace SubscriptionService.Application.UseCases.Users.Commands.RegisterUser;

public record RegisterUserCommand(
    Guid Id,
    string Email,
    DateTimeOffset CreatedWhen) : ICommand<Guid>;
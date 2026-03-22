using SubscriptionService.Application.Abstractions.Core;

namespace SubscriptionService.Application.UseCases.Users.Commands.RegisterUser;

/// <summary>Команда регистрации нового пользователя.</summary>
/// <param name="Email">Email пользователя.</param>
public record RegisterUserCommand(string Email) : ICommand<Guid>;
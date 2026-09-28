using Core.Abstractions;
using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Application.UseCases.Users.Commands.RegisterUser;

/// <summary>
/// Команда регистрации нового пользователя.
/// </summary>
public record RegisterUserCommand(RegisterUserRequest Request) : ICommand;

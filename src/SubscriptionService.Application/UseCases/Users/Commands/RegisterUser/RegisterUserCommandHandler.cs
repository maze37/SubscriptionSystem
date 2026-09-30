using Core.Database;
using CSharpFunctionalExtensions;
using SharedKernel;
using Core.Abstractions;
using SubscriptionService.Application.Abstractions;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Domain.Aggregates.User;
using SubscriptionService.Domain.ValueObjects;

namespace SubscriptionService.Application.UseCases.Users.Commands.RegisterUser;

/// <summary>
/// Обработчик команды RegisterUserCommand.
/// Проверяет уникальность email, создаёт пользователя и сохраняет.
/// </summary>
public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, RegisterUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly IDateTimeProvider _dateTime;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        ITransactionManager transactionManager,
        IDateTimeProvider dateTime)
    {
        _userRepository = userRepository;
        _transactionManager = transactionManager;
        _dateTime = dateTime;
    }

    /// <inheritdoc/>
    public async Task<Result<RegisterUserResponse, Error>> HandleAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var emailResult = UserEmail.Create(command.Request.Email);
        if (emailResult.IsFailure)
            return emailResult.Error;

        var emailExists = await _userRepository.ExistsByEmailAsync(emailResult.Value, cancellationToken);
        if (emailExists)
            return GeneralErrors.AlreadyExists("Пользователь", command.Request.Email);

        var user = User.Create(
            Guid.NewGuid(),
            emailResult.Value,
            _dateTime.UtcNow);

        _userRepository.Add(user);

        var saveResult = await _transactionManager
            .SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        return new RegisterUserResponse(user.Id);
    }
}

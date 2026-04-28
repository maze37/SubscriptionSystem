using SubscriptionService.Application.Abstractions;
using SubscriptionService.Application.Abstractions.Core;
using SubscriptionService.Domain.Aggregates.User;
using SharedKernel.Result;

namespace SubscriptionService.Application.UseCases.Users.Commands.RegisterUser;

/// <summary>
/// Обработчик команды RegisterUserCommand.
/// Проверяет уникальность email, создаёт пользователя и сохраняет.
/// </summary>
public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTime;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTime)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _dateTime = dateTime ?? throw new ArgumentNullException(nameof(dateTime));
    }

    /// <inheritdoc/>
    public async Task<Result<Guid, Error>> Handle(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var emailExists = await _userRepository
            .ExistsByEmailAsync(command.Email, cancellationToken)
            .ConfigureAwait(false);

        if (emailExists)
            return Result<Guid, Error>.Failure(
                Error.Conflict($"Пользователь с email '{command.Email}' уже существует."));

        var user = User.Create(
            Guid.NewGuid(),
            command.Email,
            _dateTime.UtcNow);

        _userRepository.Add(user);

        await _unitOfWork
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        return Result<Guid, Error>.Success(user.Id);
    }
}
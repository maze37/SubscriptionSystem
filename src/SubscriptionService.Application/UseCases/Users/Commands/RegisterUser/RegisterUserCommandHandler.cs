using SubscriptionService.Application.Abstractions;
using SubscriptionService.Application.Abstractions.Core;
using SharedKernel.Result;
using SubscriptionService.Domain.Aggregates.User;

namespace SubscriptionService.Application.UseCases.Users.Commands.RegisterUser;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<Guid>> Handle(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var emailExists = await _userRepository
            .ExistsByEmailAsync(command.Email, cancellationToken)
            .ConfigureAwait(false);

        if (emailExists)
            return Result<Guid>.Failure(
                Error.Conflict($"Пользователь с email '{command.Email}' уже существует."));

        var user = User.Create(
            command.Id,
            command.Email,
            command.CreatedWhen);

        _userRepository.Add(user);

        await _unitOfWork
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        return Result<Guid>.Success(user.Id);
    }
}
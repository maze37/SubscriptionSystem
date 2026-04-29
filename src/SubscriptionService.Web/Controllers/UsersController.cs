using MediatR;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.UseCases.Users.Commands.RegisterUser;
using SubscriptionService.Web.Contracts;
using SubscriptionService.Web.Extensions;

namespace SubscriptionService.Web.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    /// <summary>Зарегистрировать нового пользователя.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(EndpointEnvelope<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new RegisterUserCommand(request.Email);

        var result = await _sender.Send(command, cancellationToken)
            .ConfigureAwait(false);

        return this.FromResult(result, StatusCodes.Status201Created);
    }
}

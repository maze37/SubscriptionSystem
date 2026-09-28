using Core.Abstractions;
using Framework.ResponseExtensions;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.UseCases.Users.Commands.RegisterUser;

namespace SubscriptionService.Web.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly ICommandHandler<RegisterUserCommand, RegisterUserResponse> _registerUserHandler;

    public UsersController(ICommandHandler<RegisterUserCommand, RegisterUserResponse> registerUserHandler)
    {
        _registerUserHandler = registerUserHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(request);
        var response = await _registerUserHandler.HandleAsync(command, cancellationToken);

        if (response.IsFailure)
            return response.Error.ToResponse();

        return Ok(Envelope.Ok(response.Value));
    }
}

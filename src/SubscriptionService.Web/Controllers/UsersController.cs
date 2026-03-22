using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Result;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.UseCases.Users.Commands.RegisterUser;

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
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new RegisterUserCommand(request.Email);

        var result = await _sender.Send(command, cancellationToken)
            .ConfigureAwait(false);

        if (result.IsFailure)
        {
            var error = result.Errors.First();
            return error.Type switch
            {
                ErrorType.Conflict => Conflict(result.Errors),
                ErrorType.Validation => BadRequest(result.Errors),
                _ => StatusCode(500, result.Errors)
            };
        }

        return CreatedAtAction(nameof(Register), new { id = result.Value }, result.Value);
    }
}
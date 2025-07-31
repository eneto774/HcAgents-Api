using HcAgents.Domain.Commands;
using HcAgents.Domain.Entities;
using HcAgents.Domain.Models.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HcAgents.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<User>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<User>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<User>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<User>>> CreateUser(
        [FromBody] CreateUserCommand command,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var createdUser = await _mediator.Send(command, cancellationToken);

            return StatusCode(
                StatusCodes.Status201Created,
                new ApiResponse<User>(
                    true,
                    StatusCodes.Status201Created,
                    "User created",
                    createdUser
                )
            );
        }
        catch (Exception ex)
        {
            switch (ex.Message)
            {
                case "UserAlreadyExists":
                    return StatusCode(
                        StatusCodes.Status409Conflict,
                        new ApiResponse<User>(
                            false,
                            StatusCodes.Status409Conflict,
                            "User already exists",
                            ex.Message
                        )
                    );
                default:
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        new ApiResponse<User>(
                            false,
                            StatusCodes.Status500InternalServerError,
                            "Server Error creating user",
                            ex.Message
                        )
                    );
            }
        }
    }
}

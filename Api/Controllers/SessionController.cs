using HcAgents.Domain.Commands;
using HcAgents.Domain.Entities;
using HcAgents.Domain.Models.Base;
using HcAgents.Domain.Models.Response;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HcAgents.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class SessionController : ControllerBase
{
    private readonly IMediator _mediator;

    public SessionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("create")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<bool>>> CreateSession(
        [FromBody] CreateOtpCodeCommand command,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var createdSession = await _mediator.Send(command, cancellationToken);

            return StatusCode(
                StatusCodes.Status201Created,
                new ApiResponse<bool>(
                    true,
                    StatusCodes.Status201Created,
                    "Otp created",
                    createdSession
                )
            );
        }
        catch (Exception ex)
        {
            switch (ex.Message)
            {
                case "UserNotExists":
                    return StatusCode(
                        StatusCodes.Status404NotFound,
                        new ApiResponse<bool>(
                            false,
                            StatusCodes.Status404NotFound,
                            "User not exists",
                            ex.Message
                        )
                    );
                default:
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        new ApiResponse<bool>(
                            false,
                            StatusCodes.Status500InternalServerError,
                            "Server Error creating session",
                            ex.Message
                        )
                    );
            }
        }
    }

    [HttpPost]
    [Route("validate")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<CreateSessionResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ApiResponse<CreateSessionResponse>),
        StatusCodes.Status400BadRequest
    )]
    [ProducesResponseType(
        typeof(ApiResponse<CreateSessionResponse>),
        StatusCodes.Status500InternalServerError
    )]
    public async Task<ActionResult<ApiResponse<CreateSessionResponse>>> ValidateSession(
        [FromBody] CreateSessionCommand command,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var createdSession = await _mediator.Send(command, cancellationToken);

            return StatusCode(
                StatusCodes.Status201Created,
                new ApiResponse<CreateSessionResponse>(
                    true,
                    StatusCodes.Status201Created,
                    "Session validated",
                    createdSession
                )
            );
        }
        catch (Exception ex)
        {
            switch (ex.Message)
            {
                case "UserNotExists":
                    return StatusCode(
                        StatusCodes.Status404NotFound,
                        new ApiResponse<CreateSessionResponse>(
                            false,
                            StatusCodes.Status404NotFound,
                            "User not exists",
                            ex.Message
                        )
                    );
                case "InvalidOtp":
                    return StatusCode(
                        StatusCodes.Status400BadRequest,
                        new ApiResponse<CreateSessionResponse>(
                            false,
                            StatusCodes.Status400BadRequest,
                            "Otp code is invalid",
                            ex.Message
                        )
                    );
                default:
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        new ApiResponse<CreateSessionResponse>(
                            false,
                            StatusCodes.Status500InternalServerError,
                            "Server Error validate session",
                            ex.Message
                        )
                    );
            }
        }
    }
}

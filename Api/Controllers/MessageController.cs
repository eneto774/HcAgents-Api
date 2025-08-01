using System.ComponentModel.DataAnnotations;
using HcAgents.Domain.Commands;
using HcAgents.Domain.Entities;
using HcAgents.Domain.Models.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HcAgents.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class MessageController : ControllerBase
{
    private readonly IMediator _mediator;

    public MessageController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<Message>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<Message>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<Message>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<Message>>> SendMessage(
        [FromBody] SendUserMessageCommand command,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var createdMessage = await _mediator.Send(command, cancellationToken);

            return StatusCode(
                StatusCodes.Status201Created,
                new ApiResponse<Message>(
                    true,
                    StatusCodes.Status201Created,
                    "Message created",
                    createdMessage
                )
            );
        }
        catch (Exception ex)
        {
            switch (ex.Message)
            {
                default:
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        new ApiResponse<Message>(
                            false,
                            StatusCodes.Status500InternalServerError,
                            "Server Error creating Message",
                            ex.Message
                        )
                    );
            }
        }
    }

    [HttpGet]
    [Route("")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<Message>>), StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ApiResponse<IEnumerable<Message>>),
        StatusCodes.Status400BadRequest
    )]
    [ProducesResponseType(
        typeof(ApiResponse<IEnumerable<Message>>),
        StatusCodes.Status500InternalServerError
    )]
    public async Task<
        ActionResult<ApiResponse<IEnumerable<Message>>>
    > GetAllMessagesHistoryByChatId(
        [FromQuery, Required(ErrorMessage = "ChatId is required")] Guid chatId,
        [FromQuery] int? itensPerPage,
        [FromQuery] int? page,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var query = new GetAllMessagesHistoryByChatIdQuery
            {
                ChatId = chatId,
                ItensPerPage = itensPerPage,
                Page = page,
            };

            var messages = await _mediator.Send(query, cancellationToken);

            return StatusCode(
                StatusCodes.Status201Created,
                new ApiResponse<IEnumerable<Message>>(
                    true,
                    StatusCodes.Status201Created,
                    "Message created",
                    messages
                )
            );
        }
        catch (Exception ex)
        {
            switch (ex.Message)
            {
                default:
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        new ApiResponse<IEnumerable<Message>>(
                            false,
                            StatusCodes.Status500InternalServerError,
                            "Server Error creating Message",
                            ex.Message
                        )
                    );
            }
        }
    }
}

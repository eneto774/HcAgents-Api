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
public class ChatController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChatController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<Chat>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<Chat>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<Chat>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<Chat>>> CreateChatAnBot(
        [FromBody] CreateChatAndBotCommand command,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var createdChat = await _mediator.Send(command, cancellationToken);

            return StatusCode(
                StatusCodes.Status201Created,
                new ApiResponse<Chat>(
                    true,
                    StatusCodes.Status201Created,
                    "Chat created",
                    createdChat
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
                        new ApiResponse<Chat>(
                            false,
                            StatusCodes.Status500InternalServerError,
                            "Server Error creating Chat",
                            ex.Message
                        )
                    );
            }
        }
    }

    [HttpGet]
    [Route("")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<Chat>>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<Chat>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ApiResponse<IEnumerable<Chat>>),
        StatusCodes.Status500InternalServerError
    )]
    public async Task<ActionResult<ApiResponse<IEnumerable<Chat>>>> GetAllChatsHistoryByChatId(
        [FromQuery, Required(ErrorMessage = "UserId is required")] Guid userId,
        [FromQuery] int? itensPerPage,
        [FromQuery] int? page,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var query = new GetAllChatsByUserIdQuery
            {
                UserId = userId,
                ItensPerPage = itensPerPage,
                Page = page,
            };

            var chats = await _mediator.Send(query, cancellationToken);

            return StatusCode(
                StatusCodes.Status201Created,
                new ApiResponse<IEnumerable<Chat>>(
                    true,
                    StatusCodes.Status201Created,
                    "Chat created",
                    chats
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
                        new ApiResponse<IEnumerable<Chat>>(
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

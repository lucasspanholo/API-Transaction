using Microsoft.AspNetCore.Mvc;
using PdiAgile.Api.Models;
using PdiAgile.Api.Requests;
using Swashbuckle.AspNetCore.Annotations;

namespace PdiAgile.Api.Controllers;

[ApiController]
[Route("transacao")]
public class TransactionController : ControllerBase
{
    private readonly ILogger<TransactionController> _logger;

    public TransactionController(ILogger<TransactionController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [Consumes("application/json")]
    [SwaggerResponse(StatusCodes.Status201Created, "Transação aceita")]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Transação rejeitada por valor negativo")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Transação rejeitada por ModelState inválido")]
    public IActionResult CreateTransaction([FromBody] TransactionRequest? request)
    {
        if (request is null || !ModelState.IsValid)
        {
            _logger.LogWarning("Request inválida");
            return BadRequest();
        }
        if (request.Value!.Value < 0)
        {
            _logger.LogWarning("Rejected: negative value ({Value})", request.Value);
            return UnprocessableEntity();
        }
        if (request.DateTime!.Value.ToUniversalTime() > DateTimeOffset.UtcNow)
        {
            _logger.LogWarning("Rejected: future dateTime ({DateTime})", request.DateTime);
            return UnprocessableEntity();
        }

        TransactionStore.Store.Add(new PdiAgile.Api.Models.Transaction { Value = request.Value.Value, DateTime = request.DateTime.Value });
        _logger.LogInformation("Accepted transaction: value={Value} dateTime={DateTime}", request.Value.Value, request.DateTime.Value);
        return StatusCode(StatusCodes.Status201Created);
    }
}

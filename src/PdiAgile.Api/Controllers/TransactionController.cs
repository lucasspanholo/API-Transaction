using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PdiAgile.Api.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace PdiAgile.Api.Controllers;

[ApiController]
[Route("transacao")]
public class TransactionController : ControllerBase
{
    private static readonly ConcurrentBag<Transaction> Store = new();
    private readonly ILogger<TransactionController> _logger;

    public TransactionController(ILogger<TransactionController> logger)
    {
        _logger = logger;
    }

}

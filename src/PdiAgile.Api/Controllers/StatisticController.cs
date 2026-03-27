using Microsoft.AspNetCore.Mvc;
using PdiAgile.Api.Models;
using PdiAgile.Api.Requests;
using Swashbuckle.AspNetCore.Annotations;

namespace PdiAgile.Api.Controllers;

[ApiController]
[Route("estatistica")]
public class StatisticController : ControllerBase
{
    private readonly ILogger<StatisticController> _logger;

    public StatisticController(ILogger<StatisticController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, "Estatísticas obtidas")]
    public IActionResult GetStatistics()
    {
        var now = DateTimeOffset.UtcNow;
        var timeOffSet = now.AddSeconds(-60);

        var transactions = TransactionStore.Store
            .Where(t =>
            {
                var dateTimeUtc = t.DateTime.ToUniversalTime();
                return dateTimeUtc >= timeOffSet && dateTimeUtc <= now;
            })
            .ToList();

        if (transactions.Count == 0)
        {
            return Ok(new Statistics
            {
                Average = 0,
                Sum = 0,
                Min = 0,
                Max = 0,
                Count = 0
            });
        }

        var values = transactions.Select(t => t.Value).ToList();
        var statistics = new Statistics
        {
            Average = values.Average(),
            Sum = values.Sum(),
            Min = values.Min(),
            Max = values.Max(),
            Count = transactions.Count
        };
        return Ok(statistics);
    }
}

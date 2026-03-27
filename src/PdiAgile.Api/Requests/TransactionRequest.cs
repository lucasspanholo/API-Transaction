using System.ComponentModel.DataAnnotations;

namespace PdiAgile.Api.Models;

public class TransactionRequest
{
    [Required]
    public decimal? valor { get; set; }

    [Required]
    public DateTimeOffset? dataHora { get; set; }
}

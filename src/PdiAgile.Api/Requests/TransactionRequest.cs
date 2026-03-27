using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PdiAgile.Api.Requests;

public class TransactionRequest
{
    [Required]
    [JsonPropertyName("valor")]
    public decimal? Value { get; set; }

    [Required]
    [JsonPropertyName("dataHora")]
    public DateTimeOffset? DateTime { get; set; }
}

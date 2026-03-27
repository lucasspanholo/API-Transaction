using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using PdiAgile.Api.Models;
using Xunit;

namespace PdiAgile.Api.Tests;

public class EstatisticaEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public EstatisticaEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(_ => { });
    }

    [Fact]
    public async Task GetEstatistica_DeveConsiderarApenasUltimos60Segundos()
    {
        var client = _factory.CreateClient();

        Clear(TransactionStore.Store);

        TransactionStore.Store.Add(new Transaction { Value = 10m, DateTime = DateTimeOffset.UtcNow.AddSeconds(-70) });
        TransactionStore.Store.Add(new Transaction { Value = 20m, DateTime = DateTimeOffset.UtcNow.AddSeconds(-10) });

        var response = await client.GetAsync("/estatistica");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var root = await ReadJsonRoot(response);
        Assert.Equal(1, GetProperty(root, "count", "Count").GetInt32());
        Assert.Equal(20m, GetProperty(root, "sum", "Sum").GetDecimal());
        Assert.Equal(20m, GetProperty(root, "avg", "Avg").GetDecimal());
        Assert.Equal(20m, GetProperty(root, "min", "Min").GetDecimal());
        Assert.Equal(20m, GetProperty(root, "max", "Max").GetDecimal());
    }

    private static void Clear(System.Collections.Concurrent.ConcurrentBag<Transaction> bag)
    {
        while (bag.TryTake(out _))
        {
        }
    }

    private static async Task<JsonElement> ReadJsonRoot(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.Clone();
    }

    private static JsonElement GetProperty(JsonElement element, string camelName, string pascalName)
    {
        if (element.TryGetProperty(camelName, out var camel))
        {
            return camel;
        }

        return element.GetProperty(pascalName);
    }
}

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace PdiAgile.Api.Tests;

public class TransacaoEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TransacaoEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(_ => { });
    }

    [Fact]
    public async Task PostTransacao_DeveRetornar201_SemCorpo_QuandoValida()
    {
        var client = _factory.CreateClient();
        var content = new StringContent("{\"valor\": 123.45, \"dataHora\": \"2020-08-07T12:34:56.789-03:00\"}", Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/transacao", content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(string.IsNullOrEmpty(body));
    }

    [Fact]
    public async Task PostTransacao_DeveRetornar422_SemCorpo_QuandoValorNegativo()
    {
        var client = _factory.CreateClient();
        var content = new StringContent("{\"valor\": -1, \"dataHora\": \"2020-08-07T12:34:56.789-03:00\"}", Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/transacao", content);
        Assert.Equal((HttpStatusCode)422, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(string.IsNullOrEmpty(body));
    }

    [Fact]
    public async Task PostTransacao_DeveRetornar422_SemCorpo_QuandoFuturo()
    {
        var client = _factory.CreateClient();
        var content = new StringContent("{\"valor\": 10, \"dataHora\": \"2099-01-01T00:00:00.000-03:00\"}", Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/transacao", content);
        Assert.Equal((HttpStatusCode)422, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(string.IsNullOrEmpty(body));
    }

    [Fact]
    public async Task PostTransacao_DeveRetornar400_SemCorpo_QuandoCamposAusentes()
    {
        var client = _factory.CreateClient();
        var content = new StringContent("{}", Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/transacao", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(string.IsNullOrEmpty(body));
    }

    [Fact]
    public async Task PostTransacao_DeveRetornar400_SemCorpo_QuandoJsonInvalido()
    {
        var client = _factory.CreateClient();
        var content = new StringContent("{invalid", Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/transacao", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(string.IsNullOrEmpty(body));
    }

    [Fact]
    public async Task PostTransacao_DeveFalhar415_QuandoNaoJson()
    {
        var client = _factory.CreateClient();
        var content = new StringContent("{\"valor\":1,\"dataHora\":\"2020-01-01T00:00:00.000Z\"}", Encoding.UTF8, "text/plain");
        var response = await client.PostAsync("/transacao", content);
        Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
    }
}

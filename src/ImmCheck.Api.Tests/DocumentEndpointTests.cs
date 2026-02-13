using System.Net;
using System.Net.Http.Json;
using ImmCheck.Core.Models;

namespace ImmCheck.Api.Tests;

public class DocumentEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DocumentEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetI20s_ReturnsOkWithData()
    {
        var response = await _client.GetAsync("/api/documents/i20");
        response.EnsureSuccessStatusCode();
        var items = await response.Content.ReadFromJsonAsync<List<I20>>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
        Assert.Equal("N0001234567", items[0].sevisid);
    }

    [Fact]
    public async Task GetI20ById_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/documents/i20/1");
        response.EnsureSuccessStatusCode();
        var item = await response.Content.ReadFromJsonAsync<I20>();
        Assert.NotNull(item);
        Assert.Equal("CS", item.primaryMajor);
    }

    [Fact]
    public async Task GetI20ById_NotFound()
    {
        var response = await _client.GetAsync("/api/documents/i20/999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetDS2019s_ReturnsOkWithData()
    {
        var response = await _client.GetAsync("/api/documents/ds2019");
        response.EnsureSuccessStatusCode();
        var items = await response.Content.ReadFromJsonAsync<List<DS2019>>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task GetI94s_ReturnsOkWithData()
    {
        var response = await _client.GetAsync("/api/documents/i94");
        response.EnsureSuccessStatusCode();
        var items = await response.Content.ReadFromJsonAsync<List<I94>>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task GetPassports_ReturnsOkWithData()
    {
        var response = await _client.GetAsync("/api/documents/passport");
        response.EnsureSuccessStatusCode();
        var items = await response.Content.ReadFromJsonAsync<List<Passport>>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task GetVisas_ReturnsOkWithData()
    {
        var response = await _client.GetAsync("/api/documents/visa");
        response.EnsureSuccessStatusCode();
        var items = await response.Content.ReadFromJsonAsync<List<VisaInfo>>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task GetSponsoredStudents_ReturnsOkWithData()
    {
        var response = await _client.GetAsync("/api/documents/sponsored-student");
        response.EnsureSuccessStatusCode();
        var items = await response.Content.ReadFromJsonAsync<List<SponsoredStudentInfo>>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task PostI20_CreatesAndReturns()
    {
        var newI20 = new I20
        {
            idnumber = 200,
            sevisid = "N0009999999",
            status = "AC",
            eduLevel = "PhD",
            primaryMajor = "EE",
            datestamp = "2024-06-01"
        };
        var response = await _client.PostAsJsonAsync("/api/documents/i20", newI20);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<I20>();
        Assert.NotNull(created);
        Assert.Equal("N0009999999", created.sevisid);
    }

    [Fact]
    public async Task Swagger_ReturnsOk()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");
        response.EnsureSuccessStatusCode();
    }
}

namespace NoWasteOfMoney.Tests;

public class SmokeTests
{
    [Fact]
    public void Executor_IsFunctional()
    {
        Assert.True(true);
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsOk_WhenApiIsRunning()
    {
        var baseUrl = Environment.GetEnvironmentVariable("API_BASE_URL") ?? "http://localhost:8080";
        using var client = new HttpClient { BaseAddress = new Uri(baseUrl) };

        var response = await client.GetAsync("/health");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("healthy", body, StringComparison.OrdinalIgnoreCase);
    }
}

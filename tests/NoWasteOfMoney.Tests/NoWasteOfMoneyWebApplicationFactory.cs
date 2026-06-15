using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NoWasteOfMoney.Infrastructure.Database;

namespace NoWasteOfMoney.Tests;

public class NoWasteOfMoneyWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("Testing:InMemoryDatabaseName", Guid.NewGuid().ToString());
    }

    public void SeedDatabase(Action<DatabaseContext> seed)
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        context.Database.EnsureCreated();
        seed(context);
        context.SaveChanges();
    }
}

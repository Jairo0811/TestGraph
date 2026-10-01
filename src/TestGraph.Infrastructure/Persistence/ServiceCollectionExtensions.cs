using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TestGraph.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTestGraphPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TestGraph")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=TestGraph;Trusted_Connection=True;TrustServerCertificate=True";

        services.AddDbContext<TestGraphDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}

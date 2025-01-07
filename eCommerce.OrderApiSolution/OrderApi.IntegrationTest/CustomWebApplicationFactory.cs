using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using OrderApi.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using Testcontainers.MsSql;
using Respawn;
using Microsoft.Data.SqlClient;

namespace OrderApi.IntegrationTest
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
           .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
           .WithEnvironment("ACCEPT_EULA", "Y")
           .WithEnvironment("SA_PASSWORD", "P@ssw0rd123")
           .Build();

        private DbConnection _dbConnection = null!;

        public HttpClient HttpClient { get; private set; } = null!;

        private Respawner _respawner = null!;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var descriptor = services
                    .SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<OrderDbContext>));

                if (descriptor is not null)
                {
                    services.Remove(descriptor);
                }
                Console.WriteLine($"Using connection string: {_dbContainer.GetConnectionString()}");

                services.AddDbContext<OrderDbContext>(options =>
                {
                    options
                        .UseSqlServer(_dbContainer.GetConnectionString())
                        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                });

                Environment.SetEnvironmentVariable("ConnectionStrings:eCommerceConnectionTest", _dbContainer.GetConnectionString());
            });
        }

        public async Task ResetDatabaseAsync()
        {
            await _respawner.ResetAsync(_dbConnection);
        }

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();

            _dbConnection = new SqlConnection(_dbContainer.GetConnectionString());

            HttpClient = CreateClient();

            await _dbConnection.OpenAsync();
            await InitializeRespawnerAsync();
        }

        public new async Task DisposeAsync()
        {
            await _dbContainer.DisposeAsync();
            await _dbConnection.DisposeAsync();
        }

        private async Task InitializeRespawnerAsync()
        {
            _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
            {
                SchemasToInclude = ["dbo"],
                DbAdapter = DbAdapter.SqlServer
            });
        }
    }
}

using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Worker.Infrastructure.Health
{
    public class HealthCheck : IHealthCheck
    {
        private readonly string _connectionString;

        public HealthCheck(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {

            await using var connection = new SqlConnection(_connectionString);

            var healthCheckResultHealthy = await connection.ExecuteScalarAsync<int>("SELECT 1") == 1;

            if (healthCheckResultHealthy)
            {
                return HealthCheckResult.Healthy("The application is healthy.");
            }

            return HealthCheckResult.Unhealthy("The application is unhealthy.");
        }
    }
}

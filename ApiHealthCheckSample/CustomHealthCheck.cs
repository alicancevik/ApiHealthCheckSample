using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ApiHealthCheckSample
{
	public class CustomHealthCheck : IHealthCheck
	{
		private readonly List<Func<Task<HealthCheckResult>>> healthChecks;

		public CustomHealthCheck()
		{
			healthChecks = new List<Func<Task<HealthCheckResult>>>
			{
				CheckDatabaseConnectionAsync,
				CheckExternalServiceAsync
			};
		}

		public string Name => "custom_health_check";

		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, System.Threading.CancellationToken cancellationToken = default)
		{
			var healthCheckResults = new List<HealthCheckResult>();

			foreach (var healthCheck in healthChecks)
			{
				var result = await healthCheck.Invoke();
				healthCheckResults.Add(result);
			}

			var hasUnhealthyChecks = healthCheckResults.Any(r => r.Status != HealthStatus.Healthy);

			if (hasUnhealthyChecks)
			{
				return HealthCheckResult.Unhealthy("One or more health checks are unhealthy.", null, CreateAdditionalData(healthCheckResults));
			}

			return HealthCheckResult.Healthy("All health checks are healthy.", CreateAdditionalData(healthCheckResults));
		}

		private Task<HealthCheckResult> CheckDatabaseConnectionAsync()
		{
			bool isDatabaseConnectionHealthy = false;

			if (isDatabaseConnectionHealthy)
			{
				return Task.FromResult(HealthCheckResult.Healthy("Database connection is healthy."));
			}
			else
			{
				return Task.FromResult(HealthCheckResult.Unhealthy("Database connection is unhealthy."));
			}
		}

		private Task<HealthCheckResult> CheckExternalServiceAsync()
		{
			bool isExternalServiceHealthy = true;

			if (isExternalServiceHealthy)
			{
				return Task.FromResult(HealthCheckResult.Healthy("External service is healthy."));
			}
			else
			{
				return Task.FromResult(HealthCheckResult.Unhealthy("External service is unhealthy."));
			}
		}

		private Dictionary<string, object> CreateAdditionalData(List<HealthCheckResult> healthCheckResults)
		{
			var additionalData = new Dictionary<string, object>
			{
				{ "Timestamp", DateTime.UtcNow },
				{ "Environment", "Production" },
				{ "HealthCheckResults", healthCheckResults }
			};

			return additionalData;
		}
	}
}

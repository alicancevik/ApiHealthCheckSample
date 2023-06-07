using ApiHealthCheckSample;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Mime;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHealthChecks().AddCheck<CustomHealthCheck>("custom_health_check");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.MapControllers();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
	endpoints.MapHealthChecks("/health", new HealthCheckOptions
	{
		ResponseWriter = async (context, report) =>
		{
			var result = JsonSerializer.Serialize(new
			{
				status = report.Status.ToString(),
				data = report.Entries,
				errors = report.Entries.Where(x=> x.Value.Status == HealthStatus.Unhealthy).Count()
			});

			context.Response.ContentType = MediaTypeNames.Application.Json;
			await context.Response.WriteAsync(result);
		}
	});
});

app.Run();

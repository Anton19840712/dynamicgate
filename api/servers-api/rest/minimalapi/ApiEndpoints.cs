using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using servers_api.main;

namespace servers_api.rest.minimalapi
{
	public static class ApiEndpoints
	{
		public static void MapCommonApiEndpoints(this IEndpointRouteBuilder app, ILoggerFactory loggerFactory)
		{
			var logger = loggerFactory.CreateLogger("ApiEndpoints");

			// POST-запрос для загрузки файла - это главный endpoint
			app.MapPost("/api/servers/upload", async (
				[FromBody] JsonElement jsonBody,
				IUploadService uploadFileService,
				CancellationToken stoppingToken) =>
			{
				try
				{
					logger.LogInformation("Upload endpoint called with body: {JsonBody}", jsonBody.ToString());
					var result = await uploadFileService.ConfigureAsync(jsonBody, stoppingToken);
					logger.LogInformation("File uploaded successfully");
					return Results.Ok(result);
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "Error during file upload");
					return Results.Problem(ex.Message);
				}
			});
		}
	}
}

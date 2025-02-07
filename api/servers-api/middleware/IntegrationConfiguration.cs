using Serilog;
using servers_api.handlers;
using servers_api.main;
using servers_api.Services.Connectors;
using servers_api.Services.InternalSystems;
using servers_api.Services.Parsers;

namespace servers_api.middleware
{
	static class IntegrationConfiguration
	{
		/// <summary>
		/// Регистрация API сервисов, участвующих в процессе интеграции.
		/// </summary>
		public static IServiceCollection AddApiServices(this IServiceCollection services)
		{
			Log.Information("Регистрация API-сервисов...");

			services.AddTransient<IJsonParsingService, JsonParsingService>();
			services.AddTransient<ITeachService, TeachService>();
			services.AddTransient<ISenderService, SenderService>();
			services.AddTransient<IUploadService, UploadService>();

			services.AddTransient<IUploadHandler, UploadHandler>();

			Log.Information("API-сервисы зарегистрированы.");

			return services;
		}
	}
}

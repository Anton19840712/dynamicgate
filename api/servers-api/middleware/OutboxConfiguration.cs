using Serilog;
using servers_api.background;

namespace servers_api.middleware
{
	public static class OutboxConfiguration
	{
		public static IServiceCollection AddOutboxServices(this IServiceCollection services)
		{
			Log.Information("Регистрация OutboxProcessor...");

			// Регистрируем как IHostedService для фонового выполнения
			services.AddHostedService<OutboxBackgroundService>();

			Log.Information("OutboxProcessor зарегистрирован.");
			return services;
		}
	}
}

using Serilog;
using servers_api.background;
using servers_api.repositories;

namespace servers_api.middleware
{
	/// <summary>
	/// Класс обслуживает логику паттерна outbox.
	/// </summary>
	public static class OutboxConfiguration
	{
		public static IServiceCollection AddOutboxServices(this IServiceCollection services)
		{
			Log.Information("Регистрация OutboxProcessor...");

			// Регистрируем как IHostedService для фонового выполнения
			services.AddHostedService<OutboxBackgroundService>();
			services.AddSingleton<IOutboxRepository, MongoOutboxRepository>();

			Log.Information("OutboxProcessor зарегистрирован.");
			return services;
		}
	}
}

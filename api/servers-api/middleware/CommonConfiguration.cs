using Serilog;
using servers_api.factory.tcp.queuesconnections;

namespace servers_api.middleware
{
	static class CommonConfiguration
	{
		/// <summary>
		/// Регистрация сервисов общего назначения
		/// </summary>
		public static IServiceCollection AddCommonServices(this IServiceCollection services)
		{
			Log.Information("Регистрация базовых сервисов...");

			services.AddCors();
			services.AddHostedService<ResponseListenerService>();

			Log.Information("Базовые сервисы зарегистрированы.");

			return services;
		}
	}
}

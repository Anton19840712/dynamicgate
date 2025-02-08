using Serilog;
using servers_api.factory.abstractions;
using servers_api.factory.tcp.handlers;
using servers_api.factory.tcp.instances;

namespace servers_api.middleware
{
	static class ProtocolProcessingConfiguration
	{
		/// <summary>
		/// Регистрация factory сервисов при создании факторов tcp.
		/// </summary>
		public static IServiceCollection AddFactoryServices(this IServiceCollection services)
		{
			Log.Information("Регистрация factory сервисов...");

			services.AddTransient<IProtocolManager, ProtocolManager>();
			services.AddTransient<UpInstanceByProtocolFactory, TcpFactory>();

			//TCP:
			services.AddTransient<TcpServerInstance>();
			services.AddTransient<TcpClientInstance>();
			services.AddTransient<ITcpClientHandler, TcpClientHandler>();

			Log.Information("Factory сервисы зарегистрированы.");

			return services;
		}
	}
}

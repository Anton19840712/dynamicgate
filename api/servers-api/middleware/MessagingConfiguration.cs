using Serilog;
using servers_api.messaginghandlers.formatting;
using servers_api.messaginghandlers.sending;

namespace servers_api.middleware
{
	static class MessagingConfiguration
	{
		/// <summary>
		/// Регистрация сервисов, участвующих в отсылке и получении сообщений.
		/// </summary>
		public static IServiceCollection AddMessageServingServices(this IServiceCollection services)
		{
			Log.Information("Регистрация сервисов, обслуживающих messaging process...");

			services.AddTransient<IMessageSender, MessageSender>();
			services.AddTransient<IMessageFormatter, MessageFormatter>();

			Log.Information("Cервисы зарегистрированы.");

			return services;
		}
	}
}

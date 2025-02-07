using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Serilog;
using servers_api.factory.tcp.queuesconnections;
using servers_api.models.configurationsettings;
using servers_api.services.brokers.bpmintegration;
using servers_api.services.brokers.tcprest;

public static class RabbitConfiguration
{
	/// <summary>
	/// Регистрация RabbitMQ сервисов.
	/// </summary>
	public static IServiceCollection AddRabbitMqServices(this IServiceCollection services, IConfiguration configuration)
	{
		Log.Information("Инициализация RabbitMQ...");

		// Регистрация настроек RabbitMq в DI-контейнере
		services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMqSettings"));

		// Добавление IConnectionFactory с использованием настроек RabbitMq
		services.AddSingleton<IConnectionFactory>(provider =>
		{
			var rabbitMqSettings = provider.GetRequiredService<IOptions<RabbitMqSettings>>()?.Value;

			if (rabbitMqSettings == null)
			{
				Log.Error("Конфигурация RabbitMQ отсутствует! Проверьте настройки.");
				throw new InvalidOperationException("Конфигурация RabbitMQ отсутствует!");
			}

			if (string.IsNullOrWhiteSpace(rabbitMqSettings.HostName) ||
				rabbitMqSettings.Port == 0 ||
				string.IsNullOrWhiteSpace(rabbitMqSettings.UserName) ||
				string.IsNullOrWhiteSpace(rabbitMqSettings.Password))
			{
				Log.Error("Некорректные настройки RabbitMQ: {@Settings}", rabbitMqSettings);
				throw new InvalidOperationException("Некорректные настройки RabbitMQ! Проверьте конфигурацию.");
			}

			var factory = new ConnectionFactory
			{
				HostName = rabbitMqSettings.HostName,
				Port = rabbitMqSettings.Port,
				UserName = rabbitMqSettings.UserName,
				Password = rabbitMqSettings.Password
			};

			Log.Information("RabbitMQ настроен: {Host}:{Port}", factory.HostName, factory.Port);
			return factory;
		});

		services.AddSingleton<IRabbitMqQueueListener, RabbitMqQueueListener>();
		services.AddSingleton<IRabbitMqService, RabbitMqService>();
		services.AddTransient<IRabbitQueuesCreator, RabbitQueuesCreator>();

		Log.Information("Сервисы, взаимодействующие с сетевой шиной, зарегистрированы.");
		return services;
	}
}

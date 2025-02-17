﻿namespace servers_api.background
{
	using RabbitMQ.Client;
	using RabbitMQ.Client.Events;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	/// <summary>
	/// Фоновый процесс, который запускается на старте приложения и создает очередь, из которой будет слушать входящие сообщения при настройке интеграции.
	/// </summary>
	public class ListenerIntegrationBackgroundService : BackgroundService
	{
		private readonly IRabbitMqService _rabbitMqService;
		private readonly ILogger<ListenerIntegrationBackgroundService> _logger;

		public ListenerIntegrationBackgroundService(IRabbitMqService rabbitMqService, ILogger<ListenerIntegrationBackgroundService> logger)
		{
			_rabbitMqService = rabbitMqService;
			_logger = logger;
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			return Task.Run(() => ListenForResponses(stoppingToken), stoppingToken);
		}

		private void ListenForResponses(CancellationToken stoppingToken)
		{
			var connection = _rabbitMqService.CreateConnection(); // Получаем кэшированное соединение
			using var channel = connection.CreateModel();

			channel.QueueDeclare(queue: "response_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

			var consumer = new EventingBasicConsumer(channel);
			consumer.Received += (model, ea) =>
			{
				if (stoppingToken.IsCancellationRequested)
				{
					_logger.LogInformation("Ожидание ответа прекращено.");
					return;
				}

				var response = Encoding.UTF8.GetString(ea.Body.ToArray());
				_logger.LogInformation("Получен ответ: {Response}", response);

				// Здесь можно уведомить клиентов или выполнить другую бизнес-логику
			};

			channel.BasicConsume(queue: "response_queue", autoAck: true, consumer: consumer);

			// Поддерживаем выполнение, пока сервис активен
			while (!stoppingToken.IsCancellationRequested)
			{
				Task.Delay(1000).Wait();
			}
		}
	}
}
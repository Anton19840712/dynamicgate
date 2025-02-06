using System.Net.Sockets;
using System.Net;
using servers_api.factory.abstractions;
using servers_api.factory.tcp.instancehandlers;
using servers_api.models.responces;
using servers_api.models.internallayer.instance;
using System.Text;
using servers_api.services.brokers.bpmintegration;
using servers_api.models.queues;

namespace servers_api.factory.tcp.instances
{
	/// <summary>
	/// Tcp сервер, который продолжает отправлять сообщения после возврата ResponceIntegration.
	/// </summary>
	public class TcpServerInstance : IUpServer
	{
		private readonly ILogger<TcpServerInstance> _logger;
		private readonly ITcpServerHandler _tcpServerHandler;
		private readonly IRabbitMqQueueListener _rabbitMqQueueListener;

		public TcpServerInstance(
			ILogger<TcpServerInstance> logger,
			ITcpServerHandler tcpServerHandler,
			IRabbitMqQueueListener rabbitMqQueueListener)
		{
			_logger = logger;
			_tcpServerHandler = tcpServerHandler;
			_rabbitMqQueueListener = rabbitMqQueueListener;
		}

		public async Task<ResponceIntegration> UpServerAsync(
			ServerInstanceModel instanceModel,
			CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(instanceModel.Host))
				return new ResponceIntegration { Message = "Host cannot be null or empty.", Result = false };

			if (instanceModel.Port == 0)
			{
				_logger.LogError("Port is not specified. Unable to start the server.");
				return new ResponceIntegration { Message = "Port is not specified.", Result = false };
			}

			if (!IPAddress.TryParse(instanceModel.Host, out var ipAddress))
			{
				_logger.LogError("Invalid host address: {Host}", instanceModel.Host);
				return new ResponceIntegration { Message = "Invalid host address.", Result = false };
			}

			var listener = new TcpListener(ipAddress, instanceModel.Port);
			try
			{
				listener.Start();
				_logger.LogInformation("TCP сервер запущен на {Host}:{Port}", instanceModel.Host, instanceModel.Port);

				var serverSettings = instanceModel.ServerConnectionSettings;

				for (int attempt = 1; attempt <= serverSettings.AttemptsToFindBus; attempt++)
				{
					try
					{
						var client = await listener.AcceptTcpClientAsync(cancellationToken);
						_logger.LogInformation("Клиент подключился.");

						// Запускаем фоновую отправку SSE сообщений
						_ = Task.Run(() => SendSseMessagesAsync(client, cancellationToken), cancellationToken);

						//var results = await _rabbitMqQueueListener.StartListeningAsync("test_queue", cancellationToken);

						return new ResponceIntegration
						{
							Message = "Сервер запущен и клиент подключен.",
							Result = true
						};
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Ошибка во время подключения {Attempt}", attempt);
						await Task.Delay(serverSettings.BusReconnectDelayMs);
					}
				}

				return new ResponceIntegration { Message = "Не удалось подключиться после нескольких попыток.", Result = false };
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Критическая ошибка при запуске сервера.");
				return new ResponceIntegration { Message = "Критическая ошибка сервера.", Result = false };
			}
		}

		private async Task SendSseMessagesAsync(TcpClient client, CancellationToken cancellationToken)
		{
			try
			{
				using var stream = client.GetStream();
				var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

				int counter = 0;
				while (!cancellationToken.IsCancellationRequested && client.Connected)
				{
					string message = $"SSE event {counter++}: {DateTime.Now:HH:mm:ss}";
					await writer.WriteLineAsync(message);
					_logger.LogInformation($"Отправлено клиенту: {message}");
					await Task.Delay(2000, cancellationToken); // Отправка каждые 2 секунды
				}
			}
			catch (Exception ex)
			{
				_logger.LogWarning("Ошибка при отправке SSE сообщений: {Message}", ex.Message);
			}
		}
	}
}

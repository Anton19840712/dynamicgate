using System.Net.Sockets;
using System.Net;
using servers_api.factory.abstractions;
using servers_api.models.internallayer.instance;
using servers_api.validation;
using servers_api.messaging.sending;
using servers_api.models.response;

namespace servers_api.factory.tcp.instances
{
	/// <summary>
	/// Tcp сервер, который продолжает отправлять сообщения после возврата ResponseIntegration.
	/// </summary>
	public class TcpServerInstance : IUpServer
	{
		private readonly IMessageSender _messageSender;
		private readonly ILogger<TcpServerInstance> _logger;
		private readonly IServerInstanceFluentValidator _validator;


		public TcpServerInstance(
			IMessageSender messageSender,
			IServerInstanceFluentValidator validator,
			ILogger<TcpServerInstance> logger)
		{
			_messageSender = messageSender;
			_validator = validator;
			_logger = logger;
		}

		public async Task<ResponseIntegration> UpServerAsync(
				ServerInstanceModel instanceModel,
				CancellationToken cancellationToken = default)
		{
			var validationResponse = _validator.Validate(instanceModel);
			if (validationResponse != null)
				return validationResponse;

			var listener = new TcpListener(IPAddress.Parse(instanceModel.Host), instanceModel.Port);

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

						// TODO параметр модели нужно пробросить при настройке динамического шлюза, а не хардкодить:
						_ = Task.Run(() => _messageSender.SendMessagesToClientAsync(client, instanceModel.OutQueueName, cancellationToken), cancellationToken);

						return new ResponseIntegration
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

				return new ResponseIntegration { Message = "Не удалось подключиться после нескольких попыток.", Result = false };
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Критическая ошибка при запуске сервера.");
				return new ResponseIntegration { Message = "Критическая ошибка сервера.", Result = false };
			}
		}
	}
}

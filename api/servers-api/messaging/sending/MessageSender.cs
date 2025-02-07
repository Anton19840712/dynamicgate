using System.Net.Sockets;
using System.Text;
using servers_api.messaging.formatting;
using servers_api.services.brokers.bpmintegration;

namespace servers_api.messaging.sending
{
	public class MessageSender : IMessageSender
	{
		private readonly IRabbitMqQueueListener _rabbitMqQueueListener;
		private readonly IMessageFormatter _messageFormatter;
		private readonly ILogger<MessageSender> _logger;

		public MessageSender(
			IRabbitMqQueueListener rabbitMqQueueListener,
			IMessageFormatter messageFormatter,
			ILogger<MessageSender> logger)
		{
			_rabbitMqQueueListener = rabbitMqQueueListener;
			_messageFormatter = messageFormatter;
			_logger = logger;
		}

		public async Task SendMessagesToClientAsync(
			TcpClient client,
			string queueForListening,
			CancellationToken cancellationToken)
		{
			try
			{
				_ = _rabbitMqQueueListener.StartListeningAsync(queueForListening, cancellationToken);

				using var stream = client.GetStream();
				var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

				while (!cancellationToken.IsCancellationRequested && client.Connected)
				{
					var elements = _rabbitMqQueueListener.GetCollectedMessages();

					if (elements.Count == 0)
					{
						await Task.Delay(1000, cancellationToken);
						continue;
					}

					foreach (var message in elements)
					{
						string formattedJson = _messageFormatter.FormatJson(message.Message);
						await writer.WriteLineAsync(formattedJson);
						_logger.LogInformation("Отправлено клиенту:\n{Json}", formattedJson);
						await Task.Delay(2000, cancellationToken);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogWarning("Ошибка при отправке SSE сообщений: {Message}", ex.Message);
			}
		}
	}
}

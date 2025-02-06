using System.Net.Sockets;
using System.Net;
using servers_api.factory.abstractions;
using servers_api.factory.tcp.instancehandlers;
using servers_api.models.responces;
using servers_api.models.internallayer.instance;
using System.Text;
using servers_api.services.brokers.bpmintegration;
using servers_api.models.queues;
using System.Text.Json;
using System.Text.RegularExpressions;

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
				_ = _rabbitMqQueueListener.StartListeningAsync("test_queue", cancellationToken);

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
						string formattedJson = FormatJson(message.Message);
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

		private string FormatJson(string json)
		{
			try
			{
				// Десериализуем JSON в объект
				using var doc = JsonDocument.Parse(json);
				var options = new JsonSerializerOptions
				{
					WriteIndented = true,
					Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
				};

				// Преобразуем объект в строку с отступами и декодированием unicode
				using var ms = new MemoryStream();
				using (var writer = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = true }))
				{
					WriteFormattedJson(doc.RootElement, writer);
				}

				// Преобразуем байты в строку
				var formattedJson = Encoding.UTF8.GetString(ms.ToArray());

				// Декодируем Unicode-escape в строках
				return DecodeUnicodeEscape(formattedJson);
			}
			catch
			{
				return json; // Если JSON невалидный, просто вернуть как есть
			}
		}

		private string DecodeUnicodeEscape(string input)
		{
			// Декодируем все Unicode escape символы
			return Regex.Replace(input, @"\\u([0-9a-fA-F]{4})", match =>
			{
				return char.ConvertFromUtf32(int.Parse(match.Groups[1].Value, System.Globalization.NumberStyles.HexNumber));
			});
		}

		private void WriteFormattedJson(JsonElement element, Utf8JsonWriter writer)
		{
			if (element.ValueKind == JsonValueKind.Object)
			{
				writer.WriteStartObject();
				foreach (var property in element.EnumerateObject())
				{
					writer.WritePropertyName(property.Name);
					if (property.Name.Equals("Timestamp", StringComparison.OrdinalIgnoreCase) &&
						property.Value.ValueKind == JsonValueKind.String &&
						DateTime.TryParse(property.Value.GetString(), out var timestamp))
					{
						writer.WriteStringValue(timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
					}
					else
					{
						WriteFormattedJson(property.Value, writer);
					}
				}
				writer.WriteEndObject();
			}
			else if (element.ValueKind == JsonValueKind.Array)
			{
				writer.WriteStartArray();
				foreach (var item in element.EnumerateArray())
				{
					WriteFormattedJson(item, writer);
				}
				writer.WriteEndArray();
			}
			else
			{
				element.WriteTo(writer);
			}
		}
	}
}

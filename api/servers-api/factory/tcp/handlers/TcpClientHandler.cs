using servers_api.models.internallayer.instance;
using servers_api.models.outbox;
using servers_api.repositories;
using System.Net.Sockets;
using System.Text;

namespace servers_api.factory.tcp.handlers
{
	public class TcpClientHandler : ITcpClientHandler
	{
		private TcpClient _client;
		private CancellationTokenSource _cts;
		private NetworkStream _stream;
		private readonly ILogger<TcpClientHandler> _logger;
		private readonly IOutboxRepository _outboxRepository;
		private string _serverHost;
		private int _serverPort;
		private CancellationToken _token;
		private string _clientHost;
		private int _clientPort;

		public TcpClientHandler(ILogger<TcpClientHandler> logger, IOutboxRepository outboxRepository)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_outboxRepository = outboxRepository ?? throw new ArgumentNullException(nameof(outboxRepository));
		}

		public async Task<bool> TryConnectAsync(
			string serverHost,
			int serverPort,
			CancellationToken token,
			ClientInstanceModel instanceModel = null)
		{
			_cts?.Cancel();
			_cts = CancellationTokenSource.CreateLinkedTokenSource(token);
			_serverHost = serverHost;
			_serverPort = serverPort;
			_token = token;
			_clientHost = instanceModel?.ClientHost;
			_clientPort = instanceModel?.ClientPort ?? 0;

			try
			{
				_client?.Dispose();
				_client = new TcpClient();

				BindLocalEndpoint();  // Теперь BindLocalEndpoint не принимает параметры
				await _client.ConnectAsync(_serverHost, _serverPort).ConfigureAwait(false);

				if (_client.Connected)
				{
					_stream = _client.GetStream();
					await SendWelcomeMessageAsync().ConfigureAwait(false);
					_ = Task.Run(() => ReceiveMessagesAsync(), _token);
					return true;
				}
			}
			catch (SocketException ex)
			{
				_logger.LogError(ex, $"Ошибка подключения к {_serverHost}:{_serverPort}.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при подключении.");
			}

			return false;
		}

		private void BindLocalEndpoint()
		{
			if (!string.IsNullOrEmpty(_clientHost) && _clientPort > 0)
			{
				var localEndPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(_clientHost), _clientPort);
				_client.Client.Bind(localEndPoint);
				_logger.LogInformation($"Клиент привязан к {localEndPoint}");
			}
		}

		private async Task SendWelcomeMessageAsync()
		{
			try
			{
				if (_client?.Connected == true)
				{
					byte[] data = Encoding.UTF8.GetBytes("привет от tcp клиента");
					await _stream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
					await _stream.FlushAsync().ConfigureAwait(false);
					_logger.LogInformation("Отправлено приветственное сообщение.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка отправки приветственного сообщения.");
			}
		}

		private async Task ReceiveMessagesAsync()
		{
			var buffer = new byte[1024];

			do
			{
				try
				{
					int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, _token).ConfigureAwait(false);

					if (bytesRead == 0)
					{
						_logger.LogWarning("Соединение закрыто сервером.");
						break;
					}

					string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

					await _outboxRepository.SaveMessageAsync(new OutboxMessage
					{
						Message = message,
						Source = $"{_serverHost}:{_serverPort}"
					}).ConfigureAwait(false);

					_logger.LogInformation($"Cообщение сохранено: {message}");
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Ошибка при чтении данных.");
					break;
				}

			} while (!_token.IsCancellationRequested && _client.Connected);

			await ReconnectAsync();
		}

		public async Task MonitorConnectionAsync(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				await Task.Delay(5000, token).ConfigureAwait(false);
				if (!_client.Connected)
				{
					_logger.LogWarning("Соединение потеряно. Переподключение...");
					await ReconnectAsync();
				}
			}
		}

		private async Task ReconnectAsync()
		{
			_logger.LogInformation("Попытка переподключения...");
			while (!_token.IsCancellationRequested)
			{
				if (await TryConnectAsync(_serverHost, _serverPort, _token, new ClientInstanceModel { ClientHost = _clientHost, ClientPort = _clientPort }))
				{
					_logger.LogInformation("Соединение восстановлено.");
					return;
				}
				await Task.Delay(5000, _token).ConfigureAwait(false);
			}
		}

		public void Disconnect()
		{
			_logger.LogInformation("Отключение клиента...");
			_cts?.Cancel();
			_client?.Dispose();
		}
	}
}

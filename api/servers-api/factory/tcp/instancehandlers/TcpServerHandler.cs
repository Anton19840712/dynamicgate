using System.Net.Sockets;
using System.Text;

namespace servers_api.factory.tcp.instancehandlers
{
	public class TcpServerHandler : ITcpServerHandler
	{
		private readonly ILogger<TcpServerHandler> _logger;

		public TcpServerHandler(ILogger<TcpServerHandler> logger)
		{
			_logger = logger;
		}
		public async Task WaitForClientAsync(
			TcpListener listener,
			int BusResponseWaitTimeMs,
			CancellationToken cancellationToken)
		{
			try
			{
				// Ожидаем подключения клиента
				while (!cancellationToken.IsCancellationRequested)
				{
					try
					{
						var client = await listener.AcceptTcpClientAsync(cancellationToken);
						_logger.LogInformation("Client connected: {Client}", client.Client.RemoteEndPoint);

						// Обрабатываем клиента в отдельной задаче
						_ = Task.Run(() => HandleClientAsync(client, cancellationToken), cancellationToken);
					}
					catch (OperationCanceledException)
					{
						_logger.LogInformation("Server shutdown is requested.");
						break;
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Error accepting client connection.");
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during client connection waiting.");
			}
		}

		public async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
		{
			try
			{
				await using var stream = client.GetStream();
				var buffer = new byte[256];

				// Читаем сообщение от клиента
				int bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);
				if (bytesRead == 0)
				{
					_logger.LogWarning("Client {Client} disconnected.", client.Client.RemoteEndPoint);
					return;
				}

				var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
				_logger.LogInformation("Received message from {Client}: {Message}", client.Client.RemoteEndPoint, message);

				// Отправляем ответ
				var response = Encoding.UTF8.GetBytes("Message received.");
				await stream.WriteAsync(response.AsMemory(0, response.Length), cancellationToken);
				_logger.LogInformation("Response sent to {Client}.", client.Client.RemoteEndPoint);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error handling client {Client}.", client.Client.RemoteEndPoint);
			}
			finally
			{
				client.Close();
				_logger.LogInformation("Connection with client {Client} closed.", client.Client.RemoteEndPoint);
			}
		}
	}
}

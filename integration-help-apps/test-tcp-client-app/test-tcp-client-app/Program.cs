using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class TcpClientService : IHostedService
{
	private readonly ILogger<TcpClientService> _logger;
	private CancellationTokenSource _cts;
	private Task _clientTask;
	private const string ServerHost = "127.0.0.1"; // Адрес сервера
	private const int ServerPort = 6254; // Порт сервера

	public TcpClientService(ILogger<TcpClientService> logger)
	{
		_logger = logger;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Запуск TCP-клиента...");
		_cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		_clientTask = Task.Run(() => ConnectToServerAsync(_cts.Token), _cts.Token);
		return Task.CompletedTask;
	}

	private async Task ConnectToServerAsync(CancellationToken token)
	{
		while (!token.IsCancellationRequested)
		{
			using var client = new System.Net.Sockets.TcpClient();

			try
			{
				_logger.LogInformation($"Подключение к {ServerHost}:{ServerPort}...");
				await client.ConnectAsync(ServerHost, ServerPort);

				_logger.LogInformation("Успешное подключение!");

				using var stream = client.GetStream();
				byte[] buffer = new byte[1024];

				while (!token.IsCancellationRequested)
				{
					int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);
					if (bytesRead == 0)
					{
						_logger.LogWarning("Сервер закрыл соединение.");
						break;
					}

					string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
					_logger.LogInformation($"Получено: {message}");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError($"Ошибка: {ex.Message}");
				await Task.Delay(5000, token); // Ожидание перед повторным подключением
			}
		}
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Остановка TCP-клиента...");
		_cts?.Cancel();
		return _clientTask ?? Task.CompletedTask;
	}
}

public class Program
{
	public static async Task Main(string[] args)
	{
		Console.Title = "tcp-test-client";
		using var host = Host.CreateDefaultBuilder(args)
			.ConfigureServices(services =>
			{
				services.AddHostedService<TcpClientService>();
			})
			.ConfigureLogging(logging =>
			{
				logging.ClearProviders();
				logging.AddConsole();
			})
			.Build();

		await host.RunAsync();
	}
}


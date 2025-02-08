using servers_api.factory.abstractions;
using servers_api.factory.tcp.handlers;
using servers_api.models.internallayer.instance;
using servers_api.models.response;

public class TcpClientInstance : IUpClient
{
	private readonly ILogger<TcpClientInstance> _logger;
	private readonly ITcpClientHandler _helper;

	public TcpClientInstance(ILogger<TcpClientInstance> logger, ITcpClientHandler helper)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_helper = helper ?? throw new ArgumentNullException(nameof(helper));
	}

	public async Task<ResponseIntegration> ConnectToServerAsync(
		ClientInstanceModel instanceModel,
		string serverHost,
		int serverPort,
		CancellationToken token)
	{
		int maxAttempts = instanceModel.ClientConnectionSettings.AttemptsToFindExternalServer;
		int timeout = instanceModel.ClientConnectionSettings.ConnectionTimeoutMs;
		int retryDelay = instanceModel.ClientConnectionSettings.RetryDelayMs;

		while (!token.IsCancellationRequested)
		{
			for (int attempt = 1; attempt <= maxAttempts; attempt++)
			{
				_logger.LogInformation($"[{attempt}/{maxAttempts}] Подключение к {serverHost}:{serverPort}...");

				if (await _helper.TryConnectAsync(serverHost, serverPort, token, instanceModel).ConfigureAwait(false))
				{
					_logger.LogInformation($"Подключение к {serverHost}:{serverPort} успешно.");
					_ = Task.Run(() => _helper.MonitorConnectionAsync(token), token);
					return new ResponseIntegration { Message = "Успешное подключение", Result = true };
				}

				_logger.LogWarning($"Попытка подключения не удалась. Повтор через {timeout} мс...");
				try
				{
					await Task.Delay(timeout, token).ConfigureAwait(false);
				}
				catch (TaskCanceledException)
				{
					_logger.LogInformation("Подключение отменено.");
					return new ResponseIntegration { Message = "Попытка подключения отменена", Result = false };
				}
			}

			_logger.LogWarning($"Все попытки ({maxAttempts}) исчерпаны. Повтор через {retryDelay} мс...");
			try
			{
				await Task.Delay(retryDelay, token).ConfigureAwait(false);
			}
			catch (TaskCanceledException)
			{
				_logger.LogInformation("Перезапуск подключения отменен.");
				return new ResponseIntegration { Message = "Перезапуск подключения отменен", Result = false };
			}
		}

		_logger.LogError("Попытки подключения завершены безуспешно.");
		return new ResponseIntegration { Message = "Не удалось подключиться", Result = false };
	}
}

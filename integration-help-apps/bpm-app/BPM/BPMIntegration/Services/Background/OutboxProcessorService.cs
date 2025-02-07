using BPMIntegration.Models;
using BPMIntegration.Publishing;
using Marten;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ILogger = Serilog.ILogger;

namespace BPMIntegration.Services.Background
{
	public class OutboxProcessorService : IHostedService
	{
		private readonly IServiceScopeFactory _serviceScopeFactory;
		private readonly ILogger _logger;

		public OutboxProcessorService(
			IServiceScopeFactory serviceScopeFactory,
			ILogger logger)
		{
			_serviceScopeFactory = serviceScopeFactory;
			_logger = logger;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.Information("OutboxProcessorService запущен.");

			_ = Task.Run(async () =>
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					try
					{
						await ProcessOutboxMessagesAsync(cancellationToken);
					}
					catch (OperationCanceledException)
					{
						_logger.Information("Обработка сообщений была отменена.");
					}
					catch (Exception ex)
					{
						_logger.Error(ex, "Ошибка в процессе обработки сообщений.");
					}

					await Task.Delay(5000, cancellationToken);
				}
			}, cancellationToken);

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.Information("OutboxProcessorService остановлен.");
			return Task.CompletedTask;
		}

		private async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
		{
			using var scope = _serviceScopeFactory.CreateScope();
			var store = scope.ServiceProvider.GetRequiredService<IDocumentStore>();
			var messagePublisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();

			using var session = store.LightweightSession();
			try
			{
				var outboxMessages = session.Query<OutboxMessage>()
					.Where(m => !m.IsProcessed)
					.ToList();

				if (outboxMessages.Any())
				{
					_logger.Information("Найдено {Count} сообщений для обработки.", outboxMessages.Count);

					foreach (var message in outboxMessages)
					{
						cancellationToken.ThrowIfCancellationRequested();

						await messagePublisher.PublishAsync(message.OutQueueu, message.Payload);
						message.IsProcessed = true;
						session.Store(message);

						_logger.Information(
							"Сообщение outbox {MessageId} c incoming model id {Id} отправлено в очередь {Queue}.",
							message.Id,
							message.Payload.Id,
							message.OutQueueu);
					}

					await session.SaveChangesAsync();
					_logger.Information("Все сообщения успешно обработаны.");
				}
				else
				{
					_logger.Debug("Очередь сообщений пуста, обработка пропущена.");
				}
			}
			catch (OperationCanceledException)
			{
				_logger.Information("Обработка сообщений была отменена.");
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Ошибка при обработке сообщений в очереди.");
			}
		}
	}
}

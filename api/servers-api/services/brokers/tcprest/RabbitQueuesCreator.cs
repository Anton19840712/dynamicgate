using RabbitMQ.Client;
using servers_api.models.response;
using servers_api.services.brokers.tcprest;

public class RabbitQueuesCreator : IRabbitQueuesCreator
{
	private readonly ILogger<RabbitQueuesCreator> _logger;
	private readonly IConnectionFactory _connectionFactory;

	public RabbitQueuesCreator(ILogger<RabbitQueuesCreator> logger, IConnectionFactory connectionFactory)
	{
		_logger = logger;
		_connectionFactory = connectionFactory;
	}

	public async Task<ResponseIntegration> CreateQueuesAsync(string queueIn, string queueOut)
	{
		_logger.LogInformation("Создание очередей {QueueIn} и {QueueOut}", queueIn, queueOut);

		try
		{
			return await Task.Run(() =>
			{
				using var connection = _connectionFactory.CreateConnection();
				using var channel = connection.CreateModel();

				DeclareQueue(channel, queueIn);
				DeclareQueue(channel, queueOut);

				return new ResponseQueuesIntegration
				{
					Message = $"Очереди '{queueIn}' и '{queueOut}' успешно созданы.",
					Result = true,
					OutQueue = queueOut
				};
			});
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Ошибка при создании очередей {QueueIn} и {QueueOut}", queueIn, queueOut);
			return new ResponseQueuesIntegration { Message = ex.Message, Result = false };
		}
	}

	private void DeclareQueue(IModel channel, string queue)
	{
		channel.QueueDeclare(queue: queue, durable: false, exclusive: false, autoDelete: false, arguments: null);
		_logger.LogInformation("Очередь {Queue} создана.", queue);
	}
}

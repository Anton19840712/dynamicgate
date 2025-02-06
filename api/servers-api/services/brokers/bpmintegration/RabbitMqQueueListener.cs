using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using servers_api.models.responces;
using System.Text;
using servers_api.services.brokers.bpmintegration;

public class RabbitMqQueueListener : IRabbitMqQueueListener
{
	private readonly IConnectionFactory _connectionFactory;
	private readonly ILogger<RabbitMqQueueListener> _logger;
	private IConnection _connection;
	private IModel _channel;
	private string _queueName;
	private readonly List<ResponceIntegration> _collectedMessages = new();

	public RabbitMqQueueListener(IConnectionFactory connectionFactory, ILogger<RabbitMqQueueListener> logger)
	{
		_connectionFactory = connectionFactory;
		_logger = logger;
	}

	public async Task StartListeningAsync(string queueName, CancellationToken stoppingToken)
	{
		_queueName = queueName;

		try
		{
			_connection = _connectionFactory.CreateConnection();
			_channel = _connection.CreateModel();

			var consumer = new EventingBasicConsumer(_channel);
			consumer.Received += async (model, ea) => await HandleMessageAsync(ea, stoppingToken);

			_channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
			_logger.LogInformation("Слушатель очереди {Queue} запущен", _queueName);

			while (!stoppingToken.IsCancellationRequested)
			{
				await Task.Delay(5000, stoppingToken); // Ожидание перед обработкой следующей партии
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Ошибка при прослушивании {Queue}", _queueName);
		}
		finally
		{
			StopListening();
		}
	}

	private Task HandleMessageAsync(BasicDeliverEventArgs ea, CancellationToken stoppingToken)
	{
		var message = Encoding.UTF8.GetString(ea.Body.ToArray());

		_logger.LogInformation("Получено сообщение из очереди {Queue}: {Message}", _queueName, message);

		lock (_collectedMessages)
		{
			_collectedMessages.Add(new ResponceIntegration { Message = message, Result = true });
		}

		return Task.CompletedTask;
	}

	public void StopListening()
	{
		_channel?.Close();
		_connection?.Close();
		_logger.LogInformation("Слушатель {Queue} остановлен", _queueName);
	}

	public List<ResponceIntegration> GetCollectedMessages()
	{
		lock (_collectedMessages)
		{
			var messagesCopy = new List<ResponceIntegration>(_collectedMessages);
			_collectedMessages.Clear(); // Очищаем после выдачи
			return messagesCopy;
		}
	}
}

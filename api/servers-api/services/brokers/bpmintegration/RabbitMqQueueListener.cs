using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using servers_api.services.brokers.bpmintegration;
using System.Collections.Concurrent;
using servers_api.models.response;

/// <summary>
/// RabbitMqQueueListener с использованием промежуточной конкурентной коллекции с накоплением сообщений и последующей выдачей в созданный объект сервера.
/// Можно было передавать промежуточно поточно с использованием различных решений,
/// но это оказалось наиболее коротким. В зависимости от поведения системы возможно добавление раличных реализаций: networkstream, kafka, database and outbox. Но они на данный mvp момент излишни.
/// </summary>
public class RabbitMqQueueListener : IRabbitMqQueueListener
{
	private readonly IConnectionFactory _connectionFactory;
	private readonly ILogger<RabbitMqQueueListener> _logger;
	private IConnection _connection;
	private IModel _channel;
	private string _queueName;
	private readonly ConcurrentQueue<ResponseIntegration> _collectedMessages = new();

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

		_collectedMessages.Enqueue(new ResponseIntegration { Message = message, Result = true });

		return Task.CompletedTask;
	}

	public void StopListening()
	{
		_channel?.Close();
		_connection?.Close();
		_logger.LogInformation("Слушатель {Queue} остановлен", _queueName);
	}

	public List<ResponseIntegration> GetCollectedMessages()
	{
		var messagesList = new List<ResponseIntegration>();
		while (_collectedMessages.TryDequeue(out var message))
		{
			messagesList.Add(message);
		}
		return messagesList;
	}
}

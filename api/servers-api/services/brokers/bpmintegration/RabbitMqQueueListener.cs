using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using servers_api.models.queues;
using servers_api.services.brokers.bpmintegration;
using System.Text;
using System.Threading.Channels;
using servers_api.models.responces;

public class RabbitMqQueueListener : IRabbitMqQueueListener
{
	private readonly IConnectionFactory _connectionFactory;
	private readonly ILogger<RabbitMqQueueListener> _logger;
	private IConnection _connection;
	private IModel _channel;
	private string _queueName;
	private readonly Channel<ResponceIntegration> _responseChannel = Channel.CreateUnbounded<ResponceIntegration>();

	public RabbitMqQueueListener(IConnectionFactory connectionFactory, ILogger<RabbitMqQueueListener> logger)
	{
		_connectionFactory = connectionFactory;
		_logger = logger;
	}

	public async Task<ResponceIntegration> StartListeningAsync(string queueName, CancellationToken stoppingToken)
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

			return await _responseChannel.Reader.ReadAsync(stoppingToken);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Ошибка при прослушивании {Queue}", _queueName);
			return new ResponceIntegration { Message = $"Ошибка: {ex.Message}", Result = false };
		}
		finally
		{
			StopListening();
		}
	}

	private async Task HandleMessageAsync(BasicDeliverEventArgs ea, CancellationToken stoppingToken)
	{
		var message = Encoding.UTF8.GetString(ea.Body.ToArray());
		var mainMessage = JsonConvert.DeserializeObject<OutMessage>(message);

		if (mainMessage != null)
		{
			await _responseChannel.Writer.WriteAsync(new ResponceIntegration { Message = message, Result = true }, stoppingToken);
			_logger.LogInformation("Сообщение из {Queue}: {Message}", _queueName, message);
		}
		else
		{
			_logger.LogWarning("Некорректное сообщение из {Queue}", _queueName);
		}
	}

	public void StopListening()
	{
		_channel?.Close();
		_connection?.Close();
		_logger.LogInformation("Слушатель {Queue} остановлен", _queueName);
	}
}

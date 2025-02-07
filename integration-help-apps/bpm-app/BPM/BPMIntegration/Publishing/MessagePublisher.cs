using System.Text;
using BPMIntegration.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace BPMIntegration.Publishing
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly ILogger<MessagePublisher> _logger;
        private readonly ConnectionFactory _connectionFactory;

        public MessagePublisher(ILogger<MessagePublisher> logger)
        {
            _logger = logger;

            _connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
        }

		public async Task PublishAsync(string queueName, IntegrationEntity payload)
		{
			try
			{
				await Task.Run(() =>
				{
					using (var connection = _connectionFactory.CreateConnection())
					using (var channel = connection.CreateModel())
					{
						// Сериализуем объект IntegrationEntity в строку JSON
						var jsonString = JsonConvert.SerializeObject(payload); // или JsonSerializer.Serialize(payload)

						// Преобразуем строку JSON в массив байтов
						var body = Encoding.UTF8.GetBytes(jsonString);

						// Публикуем сообщение в очередь
						channel.BasicPublish(
							exchange: "",
							routingKey: queueName,
							basicProperties: null,
							body: body
						);

						_logger.LogInformation($"Сообщение опубликовано в очередь {queueName}: {jsonString}");
					}
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при публикации сообщения в очередь {EventType}: {Payload}", queueName, payload);
			}
		}
	}
}

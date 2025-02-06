using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

class Program
{
	private const string RabbitMqHost = "localhost"; // Хост RabbitMQ
	private const string QueueName = "test_queue"; // Название очереди
	private static readonly TimeSpan SendInterval = TimeSpan.FromSeconds(2); // Интервал отправки

	static async Task Main()
	{
		var factory = new ConnectionFactory { HostName = RabbitMqHost };
		using var connection = factory.CreateConnection();
		using var channel = connection.CreateModel();

		// Создаём очередь, если её нет
		channel.QueueDeclare(queue: QueueName,
							 durable: true,
							 exclusive: false,
							 autoDelete: false,
							 arguments: null);

		Console.WriteLine($"[INFO] Запущен пушер в RabbitMQ (очередь: {QueueName})");

		int counter = 0;

		while (true)
		{
			var message = new
			{
				Id = Guid.NewGuid(),
				Timestamp = DateTime.UtcNow,
				Index = counter++,
				Text = $"Сообщение {counter}"
			};

			var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

			var properties = channel.CreateBasicProperties();
			properties.Persistent = true; // Сообщения сохраняются после перезапуска

			channel.BasicPublish(exchange: "",
								 routingKey: QueueName,
								 basicProperties: properties,
								 body: body);

			Console.WriteLine($"[SENT] {message.Text}");

			await Task.Delay(SendInterval); // Ждём перед отправкой следующего сообщения
		}
	}
}

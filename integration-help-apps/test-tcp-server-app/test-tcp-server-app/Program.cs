using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
	const int Port = 5018;
	static async Task Main()
	{
		var listener = new TcpListener(IPAddress.Any, Port);
		listener.Start();
		Console.WriteLine($"Сервер запущен на порту {Port}");

		while (true)
		{
			TcpClient client = await listener.AcceptTcpClientAsync();
			Console.WriteLine("Клиент подключен");
			_ = HandleClientAsync(client);
		}
	}

	static async Task HandleClientAsync(TcpClient client)
	{
		using (client)
		using (var stream = client.GetStream())
		{
			int messageCount = 1; // Счётчик сообщений
			while (true)
			{
				string message = $"Привет от сервера! Сообщение номер {messageCount}";
				byte[] data = Encoding.UTF8.GetBytes(message);
				await stream.WriteAsync(data, 0, data.Length);
				Console.WriteLine($"Отправлено сообщение номер {messageCount}");
				messageCount++; // Увеличиваем счётчик сообщений
				await Task.Delay(2000); // Ждём 2 секунды
			}
		}
	}
}

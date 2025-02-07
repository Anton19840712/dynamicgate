using Serilog;

namespace servers_api.middleware
{
	/// <summary>
	/// Класс используется для предоставления возможности настройщику системы
	/// динамически задавать хост и порт самого динамического шлюза.
	/// </summary>
	public static class GateConfiguration
	{
		public static void ConfigureDynamicGate(string[] args, WebApplicationBuilder builder)
		{
			string port = args.FirstOrDefault(arg => arg.StartsWith("--port="))?.Split('=')[1];
			if (string.IsNullOrEmpty(port))
			{
				Log.Error("Порт не указан. Пример: MyApp.exe --port=5001");
				return;
			}

			string url = $"http://localhost:{port}";
			builder.WebHost.UseUrls(url);
			Log.Information("Приложение будет запущено по адресу: {Url}", url);
		}
	}
}

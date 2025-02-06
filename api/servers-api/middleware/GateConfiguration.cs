using Serilog;

namespace servers_api.middleware
{
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

using servers_api.models.internallayer.instance;
using servers_api.models.response;

namespace servers_api.factory.abstractions
{
	/// <summary>
	/// Класс, который поднимает в динамическом шлюзе
	/// согласно входящей информации либо клиент, либо сервер определенного типа протокола.
	/// </summary>
	public class ProtocolManager : IProtocolManager
	{
		private readonly UpInstanceByProtocolFactory _protocolFactory;
		private readonly ILogger<ProtocolManager> _logger;

		public ProtocolManager(UpInstanceByProtocolFactory protocolFactory, ILogger<ProtocolManager> logger)
		{
			_protocolFactory = protocolFactory;
			_logger = logger;
		}

		public async Task<ResponseIntegration> ConfigureAsync(InstanceModel instanceModel)
		{
			if (instanceModel is ClientInstanceModel clientModel)
			{
				return await ConfigureClientAsync(clientModel);
			}
			else if (instanceModel is ServerInstanceModel serverModel)
			{
				return await ConfigureServerAsync(serverModel);
			}

			return new ResponseIntegration
			{
				Message = "Неизвестный тип инстанса",
				Result = false
			};
		}

		private async Task<ResponseIntegration> ConfigureClientAsync(ClientInstanceModel clientModel)
		{
			var client = _protocolFactory.CreateClient();

			var serverHost = clientModel.ServerHostPort.Host;
			var serverPort = clientModel.ServerHostPort.Port ?? 80;

			_logger.LogInformation("Настройка клиента {Protocol} для подключения к серверу по адресу {Host}:{Port}",
				clientModel.Protocol, serverHost, serverPort);

			using var cts = new CancellationTokenSource(clientModel.ClientConnectionSettings.ConnectionTimeoutMs);
			return await client.ConnectToServerAsync(clientModel, serverHost, serverPort, cts.Token);
		}

		private async Task<ResponseIntegration> ConfigureServerAsync(ServerInstanceModel serverModel)
		{
			var server = _protocolFactory.CreateServer();
			_logger.LogInformation("Запуск сервера {Protocol} на {Host}:{Port}",
				serverModel.Protocol, serverModel.Host, serverModel.Port);

			using var cts = new CancellationTokenSource(serverModel.ServerConnectionSettings.BusIdleTimeoutMs);
			return await server.UpServerAsync(serverModel, cts.Token);
		}
	}
}

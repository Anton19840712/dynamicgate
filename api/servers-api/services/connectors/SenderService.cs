using AutoMapper;
using servers_api.factory.abstractions;
using servers_api.models.internallayer.common;
using servers_api.models.internallayer.instance;
using servers_api.models.responces;
using servers_api.Services.Connectors;

public class SenderService : ISenderService
{
	private readonly ILogger<SenderService> _logger;
	private readonly IProtocolManager _protocolManager;
	private readonly IMapper _mapper;

	public SenderService(
		ILogger<SenderService> logger,
		IProtocolManager protocolManager,
		IMapper mapper)
	{
		_logger = logger;
		_protocolManager = protocolManager;
		_mapper = mapper;
	}

	public async Task<ResponceIntegration> UpAsync(CombinedModel parsedModel, CancellationToken stoppingToken)
	{
		_logger.LogInformation(
			"Запуск UpAsync метода с протоколом: {Protocol}, роль: Сервер - {IsServer}, Клиент - {IsClient}",
			parsedModel.Protocol, parsedModel.DataOptions.IsServer, parsedModel.DataOptions.IsClient);

		// Используем AutoMapper для маппинга
		InstanceModel instanceModel = parsedModel.DataOptions.IsClient
			? _mapper.Map<ClientInstanceModel>(parsedModel)
			: _mapper.Map<ServerInstanceModel>(parsedModel);

		if (instanceModel is ClientInstanceModel clientModel)
		{
			_logger.LogInformation("Настройка клиента с хостом {Host} и портом {Port}", clientModel.ClientHost, clientModel.ClientPort);

			// Передаем всю модель в метод ConfigureAsync
			var responceIntegration = await _protocolManager.ConfigureAsync(
				clientModel);

			return responceIntegration;
		}
		else if (instanceModel is ServerInstanceModel serverModel)
		{
			_logger.LogInformation("Настройка сервера с хостом {Host} и портом {Port}", serverModel.Host, serverModel.Port);

			// Передаем всю модель в метод ConfigureAsync
			var responceIntegration = await _protocolManager.ConfigureAsync(
				serverModel);

			return responceIntegration;
		}

		return new ResponceIntegration
		{
			Message = "Настройка протокола не была завершена успешно",
			Result = false
		};
	}
}

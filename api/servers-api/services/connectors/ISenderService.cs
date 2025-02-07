using servers_api.models.internallayer.common;
using servers_api.models.response;

namespace servers_api.Services.Connectors
{
	/// <summary>
	/// Интерфейс главного сервиса для настройки подключения согласно указанного протокола.
	/// </summary>
	public interface ISenderService
	{
		Task<ResponseIntegration> UpAsync(
			CombinedModel parsedModel,
			CancellationToken stoppingToken);
	}
}
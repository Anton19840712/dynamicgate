using servers_api.models.response;

namespace servers_api.services.brokers.tcprest
{
	/// <summary>
	/// Создатель очередей для проведения обучения и интеграции.
	/// </summary>
	public interface IRabbitQueuesCreator
	{
		Task<ResponseIntegration> CreateQueuesAsync(string inQueue, string outQueue);
	}
}

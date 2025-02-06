using servers_api.models.responces;

namespace servers_api.services.brokers.tcprest
{
	/// <summary>
	/// Создатель очередей для проведения обучения и интеграции.
	/// </summary>
	public interface IRabbitQueuesCreator
	{
		Task<ResponceIntegration> CreateQueuesAsync(string inQueue, string outQueue);
	}
}

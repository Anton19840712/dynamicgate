using servers_api.models.internallayer.instance;

namespace servers_api.factory.tcp.handlers
{
	public interface ITcpClientHandler
	{
		void Disconnect();
		Task MonitorConnectionAsync(CancellationToken token);
		Task<bool> TryConnectAsync(string serverHost, int serverPort, CancellationToken token, ClientInstanceModel instanceModel = null);
	}
}
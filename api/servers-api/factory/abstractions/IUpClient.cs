using servers_api.models.internallayer.instance;
using servers_api.models.responces;

namespace servers_api.factory.abstractions
{
	/// <summary>
	/// Интерфейс создания instance as a client node.
	/// </summary>
	public interface IUpClient
	{
		Task<ResponceIntegration> ConnectToServerAsync(
			ClientInstanceModel instanceModel,
			string serverHost,
			int serverPort,
			CancellationToken cancellationToken);
	}
}

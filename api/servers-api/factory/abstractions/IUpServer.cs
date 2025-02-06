using servers_api.models.internallayer.instance;
using servers_api.models.responces;

namespace servers_api.factory.abstractions
{
	public interface IUpServer
	{
		Task<ResponceIntegration> UpServerAsync(
			ServerInstanceModel instanceModel,
			CancellationToken cancellationToken);
	}
}

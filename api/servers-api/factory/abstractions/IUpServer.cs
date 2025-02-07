using servers_api.models.internallayer.instance;
using servers_api.models.response;

namespace servers_api.factory.abstractions
{
	public interface IUpServer
	{
		Task<ResponseIntegration> UpServerAsync(
			ServerInstanceModel instanceModel,
			CancellationToken cancellationToken);
	}
}

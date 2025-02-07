using servers_api.models.internallayer.instance;
using servers_api.models.response;

namespace servers_api.validation
{
	public interface IServerInstanceFluentValidator
	{
		ResponseIntegration Validate(ServerInstanceModel instanceModel);
	}
}

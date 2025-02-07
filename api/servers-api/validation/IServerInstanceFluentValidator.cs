using servers_api.models.internallayer.instance;
using servers_api.models.responces;

namespace servers_api.validation
{
	public interface IServerInstanceFluentValidator
	{
		ResponceIntegration Validate(ServerInstanceModel instanceModel);
	}
}

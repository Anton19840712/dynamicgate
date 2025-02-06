using servers_api.models.internallayer.instance;
using servers_api.models.responces;

namespace servers_api.factory.abstractions
{
	public interface IProtocolManager
	{
		Task<ResponceIntegration> ConfigureAsync(InstanceModel instanceModel);
	}
}

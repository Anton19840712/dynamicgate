using servers_api.models.response;

namespace servers_api.handlers
{
	public interface IUploadHandler
	{
		public List<ResponseIntegration> GenerateResultMessage(
				ResponseIntegration queueCreationTask = null,
				ResponseIntegration senderConnectionTask = null,
				ResponseIntegration pushTask = null,
				ResponseIntegration receiveTask = null);
	}
}

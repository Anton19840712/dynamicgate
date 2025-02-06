using servers_api.models.responces;

namespace servers_api.Handlers
{
	public interface IUploadHandler
	{
		public List<ResponceIntegration> GenerateResultMessage(
				ResponceIntegration queueCreationTask = null,
				ResponceIntegration senderConnectionTask = null,
				ResponceIntegration pushTask = null,
				ResponceIntegration receiveTask = null);
	}
}

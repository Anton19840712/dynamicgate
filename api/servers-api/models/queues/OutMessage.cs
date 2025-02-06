using Newtonsoft.Json.Linq;

namespace servers_api.models.queues
{
	/// <summary>
	/// Сообщение, получаемое из сетевой шины.
	/// </summary>
	public class OutMessage
	{
		public Guid Id { get; set; }
		public string InQueueName { get; set; }
		public string OutQueueName { get; set; }
		public JObject IncomingModel { get; set; }
	}
}

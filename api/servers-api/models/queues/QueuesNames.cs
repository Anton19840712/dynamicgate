namespace servers_api.models.queues
{
	/// <summary>
	/// Очереди шины, которые будут созданы для дальнейшей работы с in и out сообщениями.
	/// </summary>
	public class QueuesNames
	{
		public string InQueueName { get; set; }
		public string OutQueueName { get; set; }
	}
}

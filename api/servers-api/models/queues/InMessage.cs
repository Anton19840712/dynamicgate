namespace servers_api.models.queues
{
	/// <summary>
	/// Сообщение, которое планируется передаваться через сетевую шину при тестировании.
	/// </summary>
	public class InMessage
	{
		public string InternalModel { get; set; }
		public QueuesNames QueuesNames { get; set; }
	}
}

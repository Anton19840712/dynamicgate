namespace servers_api.models.responces
{
	/// <summary>
	/// Частная модель для работы с возвратом информации из сервиса по созданию очередей.
	/// </summary>
	public class ResponceQueuesIntegration : ResponceIntegration
	{
		public string OutQueue { get; set; }
	}
}

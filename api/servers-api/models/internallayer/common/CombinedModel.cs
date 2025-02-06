using servers_api.models.configurationsettings;

namespace servers_api.models.internallayer.common
{
	/// <summary>
	/// Модель для пересылки на bpm для ее обучения работе с новыми структурами данных.
	/// </summary>
	public class CombinedModel
	{
		public string InQueueName { get; set; }
		public string OutQueueName { get; set; }
		public string Protocol { get; set; }
		public string InternalModel { get; set; }
		public DataOptions DataOptions { get; set; }
		public ConnectionSettings ConnectionSettings { get; set; }
		public string DataFormat { get; set; }
	}
}

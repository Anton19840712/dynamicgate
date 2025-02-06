using System.Text.Json.Serialization;

namespace servers_api.models.configurationsettings
{
	public class ClientSettings : BaseConnectionSettings
	{
		[JsonPropertyName("attemptsToFindExternalServer")]
		public int AttemptsToFindExternalServer { get; set; }

		[JsonPropertyName("connectionTimeoutMs")]
		public int ConnectionTimeoutMs { get; set; }

		[JsonPropertyName("retryDelayMs")]
		public int RetryDelayMs { get; set; }
	}
}

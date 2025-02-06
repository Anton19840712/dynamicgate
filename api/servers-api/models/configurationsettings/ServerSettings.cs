using System.Text.Json.Serialization;

namespace servers_api.models.configurationsettings
{
	public class ServerSettings : BaseConnectionSettings
	{
		[JsonPropertyName("clientHoldConnectionMs")]
		public int ClientHoldConnectionMs { get; set; }
	}
}

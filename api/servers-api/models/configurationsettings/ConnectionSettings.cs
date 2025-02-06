using System.Text.Json.Serialization;

namespace servers_api.models.configurationsettings
{
	public class ConnectionSettings
	{
		[JsonPropertyName("clientSettings")]
		public ClientSettings ClientConnectionSettings { get; set; }

		[JsonPropertyName("serverSettings")]
		public ServerSettings ServerConnectionSettings { get; set; }
	}
}

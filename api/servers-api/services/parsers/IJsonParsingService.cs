using System.Text.Json;
using servers_api.models.internallayer.common;

namespace servers_api.Services.Parsers
{
	/// <summary>
	/// Парсер входящей информации from upload endpoint.
	/// </summary>
	public interface IJsonParsingService
	{
		CombinedModel ParseJson(JsonElement jsonBody);
	}
}
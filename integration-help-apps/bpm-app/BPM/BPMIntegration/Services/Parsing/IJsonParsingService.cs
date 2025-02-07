using System.Text.Json;
using BPMIntegration.Models;

namespace BPMIntegration.Services.Parsing
{
	public interface IJsonParsingService
	{
		IntegrationEntity ParseJson(JsonElement jsonBody);
	}
}
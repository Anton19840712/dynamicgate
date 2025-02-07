using System.Text.Json;
using BPMIntegration.Models;

namespace BPMIntegration.Services.Save
{
	public interface ISaveService
	{
		Task<IntegrationEntity> SaveModelAsync(JsonElement model);
	}
}
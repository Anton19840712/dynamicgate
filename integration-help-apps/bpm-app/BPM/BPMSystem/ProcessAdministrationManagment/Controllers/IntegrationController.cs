using System.Text.Json;
using BPMIntegration.Models;
using BPMIntegration.Services.Save;
using Microsoft.AspNetCore.Mvc;

namespace BPMSystem.ProcessAdministrationManagment.Controllers
{
	[Route($"Integration")]
    [ApiController]
    public class IntegrationController : ControllerBase
    {
        private readonly ISaveService _integrationService;

        public IntegrationController(ISaveService integrationService)
        {
            _integrationService = integrationService;
        }

        [HttpGet($"ping")]
        public async Task<ActionResult<ResponceIntegration>> PingAsync()
        {
            await Task.Delay(1000);

            return new ResponceIntegration
            {
                Message = "Ping successful",
                Result = true
            };
        }

        [HttpPost($"save")]
        public async Task<ActionResult> SaveModelAsync([FromBody] JsonElement model)
        {
            var integration = await _integrationService.SaveModelAsync(model);
            return Ok(new { message = nameof(model), id = integration.Id, data = integration });
        }
    }
}

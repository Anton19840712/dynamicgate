using Microsoft.AspNetCore.Mvc;

namespace servers_api.rest.controllers
{
	[Route("/test")]
	public class TestController : ControllerBase
	{
		private readonly ILogger<TestController> _logger;

		public TestController(ILogger<TestController> logger)
		{
			_logger = logger;
		}

		[HttpGet]
		[Route("message")]
		public IActionResult Message()
		{
			_logger.LogInformation("Message endpoint was called.");
			return Ok("Hello, this is test controller.");
		}
	}
}

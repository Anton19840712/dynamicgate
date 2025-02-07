using System.Text.Json;
using BPMIntegration.Models;
using BPMIntegration.Services.Parsing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class JsonParsingService : IJsonParsingService
{
	private readonly ILogger<JsonParsingService> _logger;
	public JsonParsingService(ILogger<JsonParsingService> logger)
	{
		_logger = logger;
	}
	public IntegrationEntity ParseJson(JsonElement jsonBody)
	{
		try
		{
			_logger.LogInformation("Начало парсинга JSON.");

			// Проверяем наличие обязательных полей
			if (!jsonBody.TryGetProperty("QueuesNames", out var queuesNames) ||
				!queuesNames.TryGetProperty("InQueueName", out var inQueueName) ||
				!queuesNames.TryGetProperty("OutQueueName", out var outQueueName) ||
				!jsonBody.TryGetProperty("InternalModel", out var internalModel))
			{
				_logger.LogError("Отсутствуют обязательные поля в JSON: QueuesNames, InQueueName, OutQueueName или InternalModel.");
				throw new ArgumentException("Пропущены необходимые поля JSON");
			}

			var internalModelString = internalModel.GetString();

			var parsedObjectу = JsonConvert.DeserializeObject<JObject>(internalModelString);

			_logger.LogInformation("Парсинг строки JSON завершен.");

			// Создаем объект IntegrationEntity с декодированным JSON
			var integration = new IntegrationEntity
			{
				InQueueName = inQueueName.GetString(),
				OutQueueName = outQueueName.GetString(),
				IncomingModel = parsedObjectу // Преобразуем object в JObject
			};

			_logger.LogInformation("Парсинг JSON завершен успешно.");
			return integration;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Ошибка при парсинге JSON.");
			throw;  // Прокидываем исключение дальше
		}
	}
}
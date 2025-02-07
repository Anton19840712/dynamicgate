using System.Text;
using Newtonsoft.Json;
using servers_api.models.internallayer.common;
using servers_api.models.queues;
using servers_api.models.response;

namespace servers_api.Services.InternalSystems
{
	/// <summary>
	/// Сервис, ответственный за обучение bpm работе с новыми структурами сообщений.
	/// </summary>
	public class TeachService : ITeachService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ILogger<TeachService> _logger;

		public TeachService(
			IHttpClientFactory httpClientFactory,
			ILogger<TeachService> logger)
		{
			_httpClientFactory = httpClientFactory;
			_logger = logger;
		}

		public async Task<ResponseIntegration> TeachBPMNAsync(CombinedModel parsedModel, CancellationToken token)
		{
			_logger.LogInformation("Начало обработки TeachBPMNAsync");

			var modelForCardSystem = new InMessage
			{
				InternalModel = parsedModel.InternalModel,
				QueuesNames = new QueuesNames
				{
					InQueueName = parsedModel.InQueueName,
					OutQueueName = parsedModel.OutQueueName
				}
			};

			var client = _httpClientFactory.CreateClient();

			try
			{
				// Сериализация объекта в JSON
				var jsonContent = new StringContent(
					JsonConvert.SerializeObject(modelForCardSystem),
					Encoding.UTF8,
					"application/json"
				);

				_logger.LogInformation("Отправка POST-запроса на https://localhost:7054/Integration/save");

				// Отправляем POST-запрос с телом
				// Эти адреса необходимо сделать динамическими
				var response = await client.PostAsync("https://localhost:7054/Integration/save", jsonContent, token);

				if (response.IsSuccessStatusCode)
				{
					_logger.LogInformation("Соединение успешно установлено с API, статус-код: {StatusCode}", response.StatusCode);
					return new ResponseIntegration
					{
						Message = "API доступен, соединение успешно установлено.",
						Result = true
					};
				}
				else
				{
					_logger.LogWarning("API недоступен, статус-код: {StatusCode}", response.StatusCode);
					return new ResponseIntegration
					{
						Message = $"API недоступен. Статус-код: {(int)response.StatusCode}",
						Result = false
					};
				}
			}
			catch (HttpRequestException ex)
			{
				_logger.LogError(ex, "Ошибка при обращении к API");
				return new ResponseIntegration
				{
					Message = $"Ошибка при обращении к API: {ex.Message}",
					Result = false
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Неожиданная ошибка при проверке статуса API");
				return new ResponseIntegration
				{
					Message = $"Неожиданная ошибка при проверке статуса API: {ex.Message}",
					Result = false
				};
			}
		}
	}
}

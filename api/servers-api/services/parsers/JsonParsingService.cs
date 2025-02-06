using System.Text.Json;
using Newtonsoft.Json;
using System.Xml;
using servers_api.Services.Parsers;
using JsonSerializer = System.Text.Json.JsonSerializer;
using JsonException = System.Text.Json.JsonException;
using servers_api.models.configurationsettings;
using servers_api.models.internallayer.common;
public class JsonParsingService : IJsonParsingService
{
	private readonly ILogger<JsonParsingService> _logger;

	public JsonParsingService(ILogger<JsonParsingService> logger)
	{
		_logger = logger;
	}
	/// <summary>
	/// Реализация парсера входящих моделей.
	/// Внутренняя модель внутри входящей должна быть передана в teach service, который будет ее отсылать в bpm.
	/// Сервис работает с json и xml форматами данных.
	/// </summary>
	/// <param name="jsonBody"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentException"></exception>
	/// <exception cref="ApplicationException"></exception>
	public CombinedModel ParseJson(JsonElement jsonBody)
	{
		_logger.LogInformation("Начало разбора JSON");

		try
		{
			// Проверка наличия всех обязательных полей
			if (!jsonBody.TryGetProperty("protocol", out var protocolElement) ||
				!jsonBody.TryGetProperty("dataFormat", out var formatData) ||
				!jsonBody.TryGetProperty("companyName", out var companyNameElement) ||
				!jsonBody.TryGetProperty("model", out var modelElement) ||
				!jsonBody.TryGetProperty("dataOptions", out var dataOptionsElement) ||
				!jsonBody.TryGetProperty("connectionSettings", out var connectionSettingsElement))
			{
				_logger.LogWarning("Пропущены обязательные поля JSON");
				throw new ArgumentException("Пропущены обязательные поля JSON");
			}

			// Десериализация вложенных объектов в connectionSettings
			var clientSettings = JsonSerializer.Deserialize<ClientSettings>(connectionSettingsElement.GetProperty("clientSettings").GetRawText());
			var serverSettings = JsonSerializer.Deserialize<ServerSettings>(connectionSettingsElement.GetProperty("serverSettings").GetRawText());

			var connectionSettings = new ConnectionSettings
			{
				ClientConnectionSettings = clientSettings,
				ServerConnectionSettings = serverSettings
			};

			// Получение простых значений
			var protocol = protocolElement.GetString();
			var dataFormat = formatData.GetString();
			var companyName = companyNameElement.GetString();
			var inQueueName = $"{companyName}_in";
			var outQueueName = $"{companyName}_out";

			// Десериализация dataOptions
			var dataOptions = JsonSerializer.Deserialize<DataOptions>(dataOptionsElement.GetRawText());

			// Обработка model в зависимости от dataFormat
			string jsonString = null;
			if (dataFormat == "xml")
			{
				// Парсим XML и конвертируем в JSON
				var xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(modelElement.ToString());
				xmlDocument.RemoveChild(xmlDocument.FirstChild); // Убираем декларацию XML

				// Конвертируем XML в JSON
				jsonString = JsonConvert.SerializeXmlNode(xmlDocument, Newtonsoft.Json.Formatting.None, true);
				_logger.LogInformation("XML успешно конвертирован в JSON");
			}
			else
			{
				// Если dataFormat не XML, считаем, что model уже JSON
				jsonString = modelElement.ToString();
				_logger.LogInformation("Модель в JSON формате успешно загружена");
			}

			// Создание комбинированной модели
			var combinedModel = new CombinedModel
			{
				Protocol = protocol,
				InQueueName = inQueueName,
				OutQueueName = outQueueName,
				InternalModel = jsonString,
				DataOptions = dataOptions,
				ConnectionSettings = connectionSettings,
				DataFormat = dataFormat,
			};

			_logger.LogInformation("JSON успешно разобран и преобразован в CombinedModel");
			return combinedModel;
		}
		catch (JsonException ex)
		{
			_logger.LogError(ex, "Ошибка при обработке JSON: неверный формат данных");
			throw new ArgumentException("Ошибка при обработке JSON: неверный формат данных.", ex);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Произошла ошибка при разборе JSON");
			throw new ApplicationException("Произошла ошибка при разборе JSON.", ex);
		}
	}
}

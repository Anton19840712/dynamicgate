using System.Text;
using servers_api.models.response;

namespace servers_api.handlers
{
	/// <summary>
	/// Класс занимается валидацией результатов каждого из процессов настройки интеграции.
	/// Если все процессы завершились успешно, возвращается список интеграций с результатами каждого процесса.
	/// </summary>
	public class UploadHandler : IUploadHandler
	{
		/// <summary>
		/// Генерирует итоговое сообщение о результатах выполнения процессов интеграции.
		/// </summary>
		/// <param name="queueCreationTask">Результат задачи создания очередей брокера</param>
		/// <param name="senderConnectionTask">Результат задачи соединения согласно выбранного протокола</param>
		/// <param name="pushTask">Результат задачи обучения BPM</param>
		/// <param name="receiveTask">Результат задачи получения данных из BPM</param>
		/// <returns>Список объектов ResponseIntegration с результатами каждого процесса</returns>
		public List<ResponseIntegration> GenerateResultMessage(
					ResponseIntegration queueCreationTask = null,
					ResponseIntegration senderConnectionTask = null,
					ResponseIntegration pushTask = null,
					ResponseIntegration receiveTask = null)
		{
			var results = new List<(string ProcessName, ResponseIntegration Response)>
			{
				("Сервис создания очередей брокера", queueCreationTask),
				("Сервис запуска соединения согласно выбранного протокола", senderConnectionTask),
				("Сервис обучения BPM", pushTask),
				("Получение данных из BPM", receiveTask)
			};

			// Создаем список для хранения результатов каждого процесса
			var responseList = new List<ResponseIntegration>();

			foreach (var (processName, response) in results)
			{
				// Генерируем объект ResponseIntegration для каждого процесса
				var resultMessage = new StringBuilder();
				if (response == null)
				{
					string stringResult = string.IsNullOrEmpty(response?.Message) ? "Нет сообщения." : response.Message;

					resultMessage.Append($"{processName}: ❌ (Неизвестный результат, сервис не ответил), Сообщение: {stringResult}");
				}
				else if (response.Result)
				{
					resultMessage.Append($"{processName}: ✅ Успешно, Сообщение: {response.Message}");
				}
				else
				{
					resultMessage.Append($"{processName}: ❌ Ошибка, Сообщение: {response.Message}");
				}

				// Добавляем результат в список
				responseList.Add(new ResponseIntegration
				{
					Result = response?.Result ?? false, // Успех процесса
					Message = resultMessage.ToString()   // Сообщение о результате процесса
				});
			}

			// Возвращаем список объектов ResponseIntegration с результатами каждого процесса
			return responseList;
		}
	}
}

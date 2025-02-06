using System.Text;
using servers_api.models.responces;

namespace servers_api.Handlers
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
		/// <returns>Список объектов ResponceIntegration с результатами каждого процесса</returns>
		public List<ResponceIntegration> GenerateResultMessage(
					ResponceIntegration queueCreationTask = null,
					ResponceIntegration senderConnectionTask = null,
					ResponceIntegration pushTask = null,
					ResponceIntegration receiveTask = null)
		{
			var results = new List<(string ProcessName, ResponceIntegration Response)>
			{
				("Сервис создания очередей брокера", queueCreationTask),
				("Сервис запуска соединения согласно выбранного протокола", senderConnectionTask),
				("Сервис обучения BPM", pushTask),
				("Получение данных из BPM", receiveTask)
			};

			// Создаем список для хранения результатов каждого процесса
			var responseList = new List<ResponceIntegration>();

			foreach (var (processName, response) in results)
			{
				// Генерируем объект ResponceIntegration для каждого процесса
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
				responseList.Add(new ResponceIntegration
				{
					Result = response?.Result ?? false, // Успех процесса
					Message = resultMessage.ToString()   // Сообщение о результате процесса
				});
			}

			// Возвращаем список объектов ResponceIntegration с результатами каждого процесса
			return responseList;
		}
	}
}

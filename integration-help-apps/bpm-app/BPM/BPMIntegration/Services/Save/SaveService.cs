using System.Text.Json;
using BPMIntegration.Models;
using BPMIntegration.Services.Parsing;
using Marten;

namespace BPMIntegration.Services.Save
{

	public class SaveService : ISaveService
	{
		private readonly IDocumentStore _store;
		private readonly IJsonParsingService _jsonParsingService;

		public SaveService(IDocumentStore store, IJsonParsingService jsonParsingService)
		{
			_store = store;
			_jsonParsingService = jsonParsingService;
		}

		public async Task<IntegrationEntity> SaveModelAsync(JsonElement jsonBody)
		{
			var parsedModel = _jsonParsingService.ParseJson(jsonBody);

			// Создание и сохранение первого объекта

			Task saveParsedModelTask = Task.Run(async () =>
			{
				using (var session = _store.LightweightSession())
				{
					session.Store(parsedModel);
					await session.SaveChangesAsync();
				}
			});
			//Создание и сохранение второго объекта
			Task saveOutboxMessageTask = Task.Run(async () =>
			{
				using (var session = _store.LightweightSession())
				{
					var outboxMessage = new OutboxMessage
					{
						Id = Guid.NewGuid(),
						EventType = EventTypes.Created,
						Payload = parsedModel,
						IsProcessed = false,
						OutQueueu = parsedModel.OutQueueName,
						CreatedAt = DateTime.UtcNow
					};

					session.Store(outboxMessage);
					await session.SaveChangesAsync();
				}
			});

			//Ожидание завершения обеих задач
			await Task.WhenAll(saveParsedModelTask, saveOutboxMessageTask);

			return parsedModel;
		}
	}
}

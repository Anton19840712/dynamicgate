using MongoDB.Bson;
using servers_api.models.outbox;

namespace servers_api.repositories
{
	public interface IOutboxRepository
	{
		Task SaveMessageAsync(OutboxMessage message);
		Task<List<OutboxMessage>> GetUnprocessedMessagesAsync();
		Task MarkMessageAsProcessedAsync(ObjectId messageId);
	}
}

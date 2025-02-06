using MongoDB.Bson;
using MongoDB.Driver;
using servers_api.models.outbox;

namespace servers_api.repositories
{
	public class MongoOutboxRepository : IOutboxRepository
	{
		private readonly IMongoCollection<OutboxMessage> _collection;

		public MongoOutboxRepository(IMongoDatabase database)
		{
			_collection = database.GetCollection<OutboxMessage>("outbox_messages");
		}

		public async Task SaveMessageAsync(OutboxMessage message)
		{
			await _collection.InsertOneAsync(message);
		}

		public async Task<List<OutboxMessage>> GetUnprocessedMessagesAsync()
		{
			return await _collection.Find(m => !m.IsProcessed).ToListAsync();
		}

		public async Task MarkMessageAsProcessedAsync(ObjectId messageId)
		{
			var update = Builders<OutboxMessage>.Update
				.Set(m => m.IsProcessed, true)
				.Set(m => m.ProcessedAt, DateTime.UtcNow);
			await _collection.UpdateOneAsync(m => m.Id == messageId, update);
		}
	}
}

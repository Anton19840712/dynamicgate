using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace servers_api.models.outbox
{
	public class OutboxMessage
	{
		[BsonId]
		public ObjectId Id { get; set; }

		[BsonElement("created_at")]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		[BsonElement("processed_at")]
		public DateTime? ProcessedAt { get; set; } // Когда сообщение обработано

		[BsonElement("message")]
		public string Message { get; set; }

		[BsonElement("source")]
		public string Source { get; set; } // IP сервера

		[BsonElement("is_processed")]
		public bool IsProcessed { get; set; } = false;
	}
}

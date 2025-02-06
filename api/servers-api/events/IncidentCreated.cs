using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace servers_api.events
{
	public class IncidentCreated
	{
		/// <summary>
		/// Это наш внутренний Id события, присваивается автоматически на уровне базы.
		/// </summary>
		[BsonId]
		public ObjectId Id { get; set; }

		[BsonElement("timestamp")]
		public DateTime Timestamp { get; set; } = DateTime.UtcNow;

		[BsonElement("message")]
		public string Message { get; set; }

		[BsonElement("source")]
		public string Source { get; set; } // Например, IP-адрес сервера
	}
}

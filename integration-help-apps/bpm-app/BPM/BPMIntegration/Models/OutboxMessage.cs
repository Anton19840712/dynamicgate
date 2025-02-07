using Marten.Schema;
namespace BPMIntegration.Models
{
    public class OutboxMessage
    {
		public Guid Id { get; set; }
        public EventTypes EventType { get; set; }
        public IntegrationEntity Payload { get; set; }
        public bool IsProcessed { get; set; }
        public string OutQueueu { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

using Newtonsoft.Json.Linq;

namespace BPMIntegration.Models
{
    public class IntegrationEntity
    {
		public Guid Id { get; set; }
        public string InQueueName { get; set; }
        public string OutQueueName { get; set; }
        public JObject IncomingModel { get; set; }
    }
}
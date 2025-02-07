
using BPMIntegration.Models;

namespace BPMIntegration.Publishing
{
    public interface IMessagePublisher
    {
        Task PublishAsync(string eventType, IntegrationEntity payload);
    }
}
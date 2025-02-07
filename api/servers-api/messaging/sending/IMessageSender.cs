using System.Net.Sockets;

namespace servers_api.messaging.sending
{
	public interface IMessageSender
	{
		Task SendMessagesToClientAsync(TcpClient client, string queueForListening, CancellationToken cancellationToken);
	}
}
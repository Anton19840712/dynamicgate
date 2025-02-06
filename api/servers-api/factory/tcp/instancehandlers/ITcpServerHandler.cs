using System.Net.Sockets;

namespace servers_api.factory.tcp.instancehandlers
{
	public interface ITcpServerHandler
	{
		Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken);
		Task WaitForClientAsync(TcpListener listener, int BusResponseWaitTimeMs, CancellationToken cancellationToken);
	}
}
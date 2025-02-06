using servers_api.factory.abstractions;
using servers_api.factory.tcp.instances;

public class TcpFactory : UpInstanceByProtocolFactory
{
	private readonly ILogger<TcpFactory> _logger;
	private readonly TcpServerInstance _tcpServer;
	private readonly TcpClientInstance _tcpClient;

	public TcpFactory(
		ILogger<TcpFactory> logger,
		TcpServerInstance tcpServer,
		TcpClientInstance tcpClient
	)
	{
		_logger = logger;
		_tcpServer = tcpServer;
		_tcpClient = tcpClient;
	}

	public override IUpServer CreateServer()
	{
		_logger.LogInformation("Creating a TcpServer instance.");
		return _tcpServer;
	}

	public override IUpClient CreateClient()
	{
		_logger.LogInformation("Creating a TcpClient instance.");
		return _tcpClient;
	}
}

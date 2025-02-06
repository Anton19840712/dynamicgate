namespace servers_api.factory.abstractions
{
	/// <summary>
	/// Абстрактный класс создания сервера или клиента согласно выбранного протокола.
	/// </summary>
	public abstract class UpInstanceByProtocolFactory
	{
	    public abstract IUpServer CreateServer();
	    public abstract IUpClient CreateClient();
	}
}

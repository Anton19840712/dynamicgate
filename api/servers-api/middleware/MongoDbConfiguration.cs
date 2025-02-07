using MongoDB.Driver;
using Serilog;
using System.Security.Authentication;

namespace servers_api.middleware
{
	static class MongoDbConfiguration
	{
		/// <summary>
		/// Регистрация MongoDB клиента и связная с работой с данной базой логика.
		/// </summary>
		public static IServiceCollection AddMongoDbServices(this IServiceCollection services, IConfiguration configuration)
		{
			Log.Information("Регистрация MongoDB...");

			var mongoSettings = configuration.GetSection("MongoDbSettings");

			var user = mongoSettings.GetValue<string>("User");
			var password = mongoSettings.GetValue<string>("Password");
			var connectionString = mongoSettings.GetValue<string>("ConnectionString");
			var databaseName = mongoSettings.GetValue<string>("DatabaseName");

			var mongoUrl = new MongoUrlBuilder(connectionString)
			{
				Username = user,
				Password = password
			}.ToString();

			var settings = MongoClientSettings.FromUrl(new MongoUrl(mongoUrl));
			settings.SslSettings = new SslSettings { EnabledSslProtocols = SslProtocols.Tls12 };

			services.AddSingleton<IMongoClient>(new MongoClient(settings));
			services.AddSingleton(sp =>
			{
				var client = sp.GetRequiredService<IMongoClient>();
				return client.GetDatabase(databaseName);
			});

			Log.Information("MongoDB зарегистрирован.");

			return services;
		}
	}
}

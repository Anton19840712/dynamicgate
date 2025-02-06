using Serilog;
using servers_api.middleware;
using servers_api.rest.minimalapi;

Console.Title = "integration api";

var builder = WebApplication.CreateBuilder(args);

// Настройка логирования
builder.Host.UseSerilog((ctx, cfg) =>
{
	cfg.WriteTo
		.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
		.Enrich
		.FromLogContext();
});

CancellationTokenSource cts = null;
try
{
	//GateConfiguration.ConfigureDynamicGate(args, builder);

	var services = builder.Services;
	var configuration = builder.Configuration;

	services.AddControllers();

	services.AddCommonServices();
	services.AddHttpServices();
	services.AddFactoryServices();
	services.AddApiServices();
	services.AddRabbitMqServices(configuration);
	services.AddMongoDbServices(configuration);
	services.AddAutoMapper(typeof(MappingProfile));
	services.AddOutboxServices();

	var app = builder.Build();

	app.UseSerilogRequestLogging();
	app.UseCors(cors => cors.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

	var factory = app.Services.GetRequiredService<ILoggerFactory>();

	app.MapControllers();
	app.MapCommonApiEndpoints(factory);

	Log.Information("Приложение запускается...");
	app.Run();
}
catch (Exception ex)
{
	Log.Fatal(ex, "Критическая ошибка при запуске приложения");
	throw;
}
finally
{
	cts.Cancel();
	cts.Dispose();

	Log.CloseAndFlush();
}

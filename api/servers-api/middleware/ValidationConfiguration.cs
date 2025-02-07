using FluentValidation;
using Serilog;
using servers_api.models.internallayer.instance;
using servers_api.validation;

namespace servers_api.middleware
{
	static class ValidationConfiguration
	{
		/// <summary>
		/// Регистрация сервисов валидации.
		/// </summary>
		public static IServiceCollection AddValidationServices(this IServiceCollection services)
		{
			Log.Information("Регистрация сервисов валидации...");

			services.AddScoped<IServerInstanceFluentValidator, ServerInstanceFluentValidator>();
			services.AddScoped<IValidator<ServerInstanceModel>, ServerInstanceModelValidator>();

			Log.Information("Сервисы валидации зарегистрированы.");

			return services;
		}
	}
}

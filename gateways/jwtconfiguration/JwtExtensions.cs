using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace jwtconfiguration
{
	public static class JwtExtensions
	{
		// если ключ будет недостаточно длинный, то словим ошибку
		// System.ArgumentOutOfRangeException:
		// 'IDX10720: Unable to create KeyedHashAlgorithm for algorithm 'HS256',
		// the key size must be greater than: '256' bits, key has '184' bits. (Parameter 'keyBytes')'
		public const string SecurityKey = "myverysecretkeythatissufficientlylongSecretJWTsigningKey@123";
		public const string ValidIssuer = "https://localhost:5004";// это локалхост и порт твоего auth приложения

		public static void AddJwtAuthentication(this IServiceCollection services)
		{
			services.AddAuthentication(opt =>
			{
				opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuer = ValidIssuer,
					ValidateAudience = false,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey))
				};
			});
		}
	}
}

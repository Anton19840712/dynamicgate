using jwtconfiguration;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration)
    .AddCacheManager(x =>
    {
        x.WithDictionaryHandle();
    });

builder.Services.AddJwtAuthentication();
//без этой строки будет ошибка:
//System.Exception: 'Unable to start Ocelot, errors are: Authentication Options AuthenticationProviderKey:
//'Bearer',AuthenticationProviderKeys:[],AllowedScopes:['products.read'] is unsupported authentication provider'

var app = builder.Build();
app.UseCors("AllowAll");
//app.UseAuthorization();
app.UseRouting();

app.UseCors("AllowAllOrigins");

app.UseAuthorization();


//app.UseAuthorization();
await app.UseOcelot();

//app.UseAuthentication();
//app.UseAuthorization();

app.Run();
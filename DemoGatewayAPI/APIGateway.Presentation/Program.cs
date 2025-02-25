using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using eCommerce.SharedLibrary.DependencyInjection;
using APIGateway.Presentation.Middleware;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("ocelot.json",optional: false, reloadOnChange: true);
builder.Services.AddOcelot().AddCacheManager(x => x.WithDictionaryHandle());
// Configure the HTTP request pipeline.
JWTAuthenticationScheme.AddJWTAuthenticationScheme(builder.Services, builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {

        builder.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});
var app = builder.Build();
app.UseHttpsRedirection();
app.UseCors();
app.UseMiddleware<AttachSignature>();
app.UseOcelot().Wait();
app.Run();

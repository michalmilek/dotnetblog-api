using System.Text.Json.Serialization;
using auth_playground.Authorization;
using auth_playground.Helpers;
using auth_playground.Services;

var builder = WebApplication.CreateBuilder(args);
{
    var services = builder.Services;
    var env = builder.Environment;

    services.AddDbContext<DataContext>();
    services.AddCors();

    services.AddControllers().AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        x.JsonSerializerOptions.WriteIndented = true;
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });
    
    services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
    services.AddScoped<IJwtUtils, JwtUtils>();
    services.AddScoped<IUserService, UserService>();
}


var app = builder.Build();

// Configure the HTTP request pipeline.
{

app.UseCors(x => x
    .SetIsOriginAllowed(origin => true)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());
app.UseMiddleware<ErrorHandlerMiddleware>();
app.MapControllers();
}

app.Run();
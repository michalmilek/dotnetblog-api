using System.Text;
using System.Text.Json.Serialization;
using auth_playground.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using auth_playground.Helpers;
using auth_playground.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
{
    var services = builder.Services;
    var env = builder.Environment;
    
    services.AddDbContext<DataContext>();
    services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
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
    services.AddScoped<IArticleService, ArticleService>();
    services.AddHostedService<TokenCleanupService>();
    
    
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["AppSettings:Secret"])),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
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
app.UseAuthentication(); // Ensure Authentication middleware is used
app.UseAuthorization();
app.MapControllers();
}

app.Run();
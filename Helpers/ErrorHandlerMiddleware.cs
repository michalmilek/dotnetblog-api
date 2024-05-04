using System.Net;
using System.Text.Json;

namespace auth_playground.Helpers;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    
    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            
            switch (e)
            {
                case AppException appException:
                    response.StatusCode = (int) HttpStatusCode.BadRequest;
                    break;
                case KeyNotFoundException keyNotFoundException:
                    response.StatusCode = (int) HttpStatusCode.NotFound;
                    break;
                case UnauthorizedAccessException unauthorizedAccessException:
                    response.StatusCode = (int) HttpStatusCode.Unauthorized;
                    break;
                default:
                    response.StatusCode = (int) HttpStatusCode.InternalServerError;
                    break;
            }
            
            Console.WriteLine(e);
            var result = JsonSerializer.Serialize(new { message = e?.Message });
            await response.WriteAsync(result);
        }
    }
}
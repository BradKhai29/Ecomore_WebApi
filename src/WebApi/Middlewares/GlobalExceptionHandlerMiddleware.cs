
using WebApi.Models;

namespace WebApi.Middlewares
{
    public class GlobalExceptionHandlerMiddleware : IMiddleware
    {
        public async Task InvokeAsync(
            HttpContext context,
            RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (Exception)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                var response = ApiResponse.Failed(ApiResponse.DefaultMessage.ServerError);

                context.Response.Clear();
                await context.Response.WriteAsJsonAsync(response);
                await context.Response.CompleteAsync();                
            };
        }
    }
}

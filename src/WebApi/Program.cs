using BusinessLogic;
using Presentation.Middlewares;
using WebApi.DependencyInjection;
using WebApi.Models;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

#region Core and External Services Configuration
services.AddDbContextConfiguration(configuration);
services.AddIdentityConfiguration();
services.AddBusinessLogic();
#endregion

#region Presentation Configuration
services.AddOptionsConfiguration(configuration);
services.AddAuthenticationConfiguration();
services.AddAuthorizationConfiguration();
services.AddWebApiConfiguration(configuration);
services.AddSignalR();
// Read more about custom exception handler: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-8.0
services.AddExceptionHandler(options =>
{
    options.ExceptionHandler = async (context) =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var response = ApiResponse.Failed(ApiResponse.DefaultMessage.ServerError);

        context.Response.Clear();
        await context.Response.WriteAsJsonAsync(response);
        await context.Response.CompleteAsync();
    };
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

using BusinessLogic;
using WebApi.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

#region Core and External Services Configuration
services.AddDbContextConfiguration(configuration);
services.AddIdentityConfiguration();
services.AddBusinessLogic();
#endregion

#region Presentation Configuration
services.AddOptionsConfiguration(configurationManager: configuration);
services.AddAuthenticationConfiguration();
services.AddAuthorizationConfiguration();
services.AddWebApiConfiguration();
services.AddSignalR();
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

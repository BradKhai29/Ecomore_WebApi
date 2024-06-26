﻿using Options.Models;
using WebApi.Shared.CustomActionResults;

namespace WebApi.DependencyInjection;

public static class WebApiConfiguration
{
    public static IServiceCollection AddWebApiConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var dataProtectionOptions = new ProtectionOptions();
        dataProtectionOptions.Bind(configuration);

        services.AddDataProtection(options =>
        {
            options.ApplicationDiscriminator = dataProtectionOptions.ApplicationDiscriminator;
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                configurePolicy: policy =>
                {
                    policy.WithOrigins("http://localhost:8080/");
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.AllowCredentials();
                });
        });

        services.AddControllers(configure: config =>
        {
            config.SuppressAsyncSuffixInActionNames = config.SuppressAsyncSuffixInActionNames;
        })
        .ConfigureApiBehaviorOptions(setupAction: config =>
        {
            config.InvalidModelStateResponseFactory = actionContext =>
            {
                return new CustomGlobalBadRequestResult();
            };
        });
        services.AddSwaggerConfiguration();
        services.ConfigCors();
        services.ConfigureLogging();

        return services;
    }

    /// <summary>
    ///     Configure the logging service.
    /// </summary>
    /// <param name="services">
    ///     Service container.
    /// </param>
    private static void ConfigureLogging(this IServiceCollection services)
    {
        services.AddLogging(configure: config =>
        {
            config.ClearProviders();
            config.AddConsole();
        });
    }

    /// <summary>
    /// Configures CORS for the web API.
    /// </summary>
    /// <param name="services">The IServiceCollection instance.</param>
    private static void ConfigCors(this IServiceCollection services)
    {
        services.AddCors(setupAction: config =>
        {
            config.AddDefaultPolicy(configurePolicy: policy =>
            {
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
        });
    }
}

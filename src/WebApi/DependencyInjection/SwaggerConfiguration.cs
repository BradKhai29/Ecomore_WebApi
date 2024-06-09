using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.DependencyInjection
{
    public static class SwaggerConfiguration
    {
        public const string SwaggerDocName = "v1";
        public const string SwaggerDocVersion = "v1";
        private const string SwaggerDocTitle = "Ecomore WebApi";
        private const string SwaggerDocDescription = "The WebApi for Ecomore app";
        private const string AuthorizationHttpHeader = "Authorization";

        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(setupAction: options =>
            {
                // Document configuration for swagger, including definition and terms.
                options.ConfigureDocument();

                options.AddSecurityConfiguration();

                options.AddXmlComments();
            });

            return services;
        }

        private static SwaggerGenOptions ConfigureDocument(this SwaggerGenOptions options)
        {
            options.SwaggerDoc(
                name: SwaggerDocName,
                info: new()
                {
                    Version = SwaggerDocVersion,
                    Title = SwaggerDocTitle,
                    Description = SwaggerDocDescription,
                    License = new()
                    {
                        Name = "MIT",
                        Url = new("https://opensource.org/license/mit/")
                    }
                }
            );

            return options;
        }

        private static SwaggerGenOptions AddSecurityConfiguration(this SwaggerGenOptions options)
        {
            options.AddSecurityDefinition(
                name: JwtBearerDefaults.AuthenticationScheme,
                securityScheme: new()
                {
                    Description =
                        @"JWT Authorization header using the Bearer scheme.
                        Enter 'Bearer' [space] and then your token in the text input below.
                        Example: 'Bearer 12345abcdef'",
                    Name = AuthorizationHttpHeader,
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                }
            );

            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            },
                            Scheme = "oauth2",
                            Name = JwtBearerDefaults.AuthenticationScheme,
                            In = ParameterLocation.Header,
                        },
                        new List<string>(0)
                    }
                }
            );

            return options;
        }

        private static SwaggerGenOptions AddXmlComments(this SwaggerGenOptions options)
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

            var xmlFilePath = Path.Combine(path1: AppContext.BaseDirectory, path2: xmlFilename);

            options.IncludeXmlComments(filePath: xmlFilePath);

            return options;
        }
    }
}

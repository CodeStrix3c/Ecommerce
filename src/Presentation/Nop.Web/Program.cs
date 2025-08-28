using Autofac.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Infrastructure.Extensions;

// ADD:
using Microsoft.OpenApi.Models;

namespace Nop.Web;

public partial class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddJsonFile(NopConfigurationDefaults.AppSettingsFilePath, true, true);
        if (!string.IsNullOrEmpty(builder.Environment?.EnvironmentName))
        {
            var path = string.Format(NopConfigurationDefaults.AppSettingsEnvironmentFilePath, builder.Environment.EnvironmentName);
            builder.Configuration.AddJsonFile(path, true, true);
        }
        builder.Configuration.AddEnvironmentVariables();

        //load application settings
        builder.Services.ConfigureApplicationSettings(builder);

        var appSettings = Singleton<AppSettings>.Instance;
        var useAutofac = appSettings.Get<CommonConfig>().UseAutofac;

        if (useAutofac)
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        else
            builder.Host.UseDefaultServiceProvider(options =>
            {
                options.ValidateScopes = false;
                options.ValidateOnBuild = true;
            });

        // NOP default registrations
        builder.Services.ConfigureApplicationServices(builder);

        // =========================
        // ADD: MVC API controllers
        // =========================
        // This sits alongside Nop's MVC; it ensures attribute-routed API controllers work (e.g., [ApiController])
        builder.Services
            .AddControllers()
            .AddNewtonsoftJson(); // keeps compatibility with Nop's Newtonsoft usage

        // =========================
        // ADD: Swagger / OpenAPI
        // =========================
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Admin APIs",
                Version = "v1",
                Description = "nopCommerce Admin/Web API surface"
            });

            // Optional: show XML comments (if you generate XML docs)
            // var xml = Path.Combine(AppContext.BaseDirectory, "Nop.Web.xml");
            // if (File.Exists(xml)) c.IncludeXmlComments(xml);

            // Optional: JWT bearer support (if you secure APIs this way)
            var scheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter: Bearer {your JWT token}"
            };
            c.AddSecurityDefinition("Bearer", scheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                [scheme] = new List<string>()
            });
        });

        var app = builder.Build();

        // =====================================
        // ADD: Swagger middleware & UI (dev)
        // =====================================
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Admin APIs v1");
                c.RoutePrefix = "swagger"; // serve at /swagger
            });
        }

        // NOP default pipeline
        app.ConfigureRequestPipeline();

        // =====================================
        // ADD: Map controllers endpoints
        // =====================================
        // Keep after ConfigureRequestPipeline so routes are ready and middlewares are in place
        app.MapControllers();

        await app.PublishAppStartedEventAsync();
        await app.RunAsync();
    }
}

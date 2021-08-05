using System.Collections.Generic;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using Serilog;
using Database;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Worker.Infrastructure.Health;
using Worker.Infrastructure.Middleware;

namespace Worker
{
    public class Startup
    {
        private readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var databaseSettings = Configuration.GetSection("Database").Get<DatabaseSettings>();
            var connectionString = databaseSettings.GetConnectionString();

            services.AddHostedService<BackgroundWorkers.Worker>();
            services.AddSingleton<IRepositoryWatcher>(new RepositoryWatcher(connectionString));

            services.AddHttpContextAccessor();
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();

            services.AddHealthChecks()
                .AddCheck("Health check", new HealthCheck(connectionString));

            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.AllowAnyOrigin();
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseCors(MyAllowSpecificOrigins);
            app.UseRouting();
 
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHttpMetrics();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
                endpoints.MapMetrics();
            });

            var databaseSettings = Configuration.GetSection("Migration").Get<DatabaseSettings>();
            DbMigrator.Migrate(databaseSettings.GetConnectionString());
        }
    }
}
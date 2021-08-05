using System;
using System.Collections.Generic;
using System.Net.Http;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Configurations.Databases;
using DotNet.Testcontainers.Containers.Modules.Databases;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Worker;

namespace SystemTests
{
    public class ApplicationFixture : IDisposable
    {
        private readonly WebApplicationFactory<Startup> _webApplicationFactory;
        private readonly MsSqlTestcontainer _testDbContainer;

        public HttpClient HttpClient;

        public ApplicationFixture()
        {
            var testContainersBuilder = new TestcontainersBuilder<MsSqlTestcontainer>()
                .WithDatabase(
                    new MsSqlTestcontainerConfiguration
                    {
                        Password = "StrongPassword1"
                    });

            _testDbContainer = testContainersBuilder.Build();
            _testDbContainer.StartAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            _webApplicationFactory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
                    {
                        options.TokenValidationParameters.ValidateLifetime = false;
                    });
                });

                builder.ConfigureAppConfiguration((c, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string>()
                    {
                        {"Migration:DataSource", _testDbContainer.Hostname},
                        {"Migration:InitialCatalog", _testDbContainer.Database},
                        {"Migration:IntegratedSecurity", false.ToString()},
                        {"Migration:UserId", _testDbContainer.Username},
                        {"Migration:Password", _testDbContainer.Password},
                        {"Migration:Port", _testDbContainer.Port.ToString()},
                        {"Database:DataSource", _testDbContainer.Hostname},
                        {"Database:InitialCatalog", _testDbContainer.Database},
                        {"Database:IntegratedSecurity", false.ToString()},
                        {"Database:UserId", _testDbContainer.Username},
                        {"Database:Password", _testDbContainer.Password},
                        {"Database:Port", _testDbContainer.Port.ToString()}
                    });
                });
                ;
            });

            HttpClient = _webApplicationFactory.CreateClient();
        }

        public void Dispose()
        {
            _webApplicationFactory?.Dispose();
            _testDbContainer.DisposeAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}

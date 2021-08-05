using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Database;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Worker.BackgroundWorkers
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IRepositoryWatcher watcher;

        public Worker(ILogger<Worker> logger, IRepositoryWatcher watcher)
        {
            _logger = logger;
            this.watcher = watcher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                List<Guid> dataToBeProcessed = await watcher.GetNewUsers();

                foreach (var item in dataToBeProcessed)
                {
                    //processing logic goes here
                    _logger.Log(LogLevel.Information, "Processed order with id {Id}", item);
                }

                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}

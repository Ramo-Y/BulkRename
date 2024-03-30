namespace BulkRename.Models
{
    using System.Diagnostics;
    using BulkRename.Models.Entities;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    public class PreparationDatabase
    {
        private readonly ILogger<PreparationDatabase> _logger;

        private readonly Stopwatch _stopwatch;

        private int _connectionTimeOut;

        public PreparationDatabase(ILogger<PreparationDatabase> logger)
        {
            _logger = logger;
            _stopwatch = new Stopwatch();
        }

        public async Task PreparatePopulation(IApplicationBuilder applicationBuilder, int connectionTimeOut)
        {
            _connectionTimeOut = connectionTimeOut;
            _stopwatch.Start();

            using (var serviceScope = applicationBuilder.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<BulkRenameContext>();

                await TrySeedData(context!);
            }
        }

        private async Task TrySeedData(DbContext context)
        {
            try
            {
                _logger.LogInformation("Trying to apply migrations...");
                var appliedMigrations = await context!.Database.GetAppliedMigrationsAsync();
                LogMigrationInformation("applied", appliedMigrations);
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                LogMigrationInformation("pending", pendingMigrations);
                await context.Database.MigrateAsync();
                await context.DisposeAsync();
                _logger.LogInformation("Migration finished");
            }
            catch (SqlException exception)
            {
                if (_stopwatch.ElapsedMilliseconds > _connectionTimeOut)
                {
                    _logger.LogError($"Connection timeout of {_connectionTimeOut}ms ellapsed! Application is shutting down...", exception);
                    throw;
                }

                _logger.LogWarning("Database is propably not ready...");
                _logger.LogWarning($"Connection failed, will try again. Max timeout = {_connectionTimeOut}ms");
                Thread.Sleep(1000);
                await TrySeedData(context);
            }
        }

        private void LogMigrationInformation(string state, IEnumerable<string> migrations)
        {
            if (migrations.Any())
            {
                _logger.LogInformation($"Following migrations are {state}");
                foreach (var migration in migrations)
                {
                    _logger.LogInformation($"{migration}", migration);
                }
            }
        }
    }
}
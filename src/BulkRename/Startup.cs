namespace BulkRename
{
    using System.Diagnostics;

    using BulkRename.Constants;
    using BulkRename.Extensions;
    using BulkRename.Interfaces;
    using BulkRename.Models;
    using BulkRename.Models.Entities;
    using BulkRename.Services;

    using Microsoft.EntityFrameworkCore;

    using Serilog;

    public class Startup
    {
        private string? _connectionString;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            SetConnectionString();

            services.AddControllersWithViews();

            services.AddDbContext<BulkRenameContext>(opt => { opt.UseSqlServer(_connectionString, builder => builder.EnableRetryOnFailure(120, TimeSpan.FromSeconds(10), null)); });
            services.AddControllers();

            // INFO: Register Services
            services.AddTransient<PreparationDatabase>();
            services.AddTransient<IFileService, FileService>();
            ConfigurePersistance(services);
        }

        private void ConfigurePersistance(IServiceCollection services)
        {
            var persistanceMode = Configuration.GetValue<PersistanceMode>(nameof(PersistanceMode));
            switch (persistanceMode)
            {
                case PersistanceMode.None:
                    services.AddTransient<IPersistanceService, EmptyPersistanceService>();
                    break;
                case PersistanceMode.Database:
                    services.AddTransient<IPersistanceService, DatabasePersistanceService>();
                    break;
                case PersistanceMode.Json:
                    services.AddTransient<IPersistanceService, JsonPersistanceService>();
                    break;
                default:
                    services.AddTransient<IPersistanceService, EmptyPersistanceService>();
                    break;
            }
        }

        public async Task Configure(WebApplication app)
        {
            ConfigureLogger();
            await PreparateDatabase(app);
        }

        private void ConfigureLogger()
        {
            var seqUrl = Configuration[ConfigurationNameConstants.SEQ_URL];

            if (!string.IsNullOrEmpty(seqUrl))
            {
                var seqApiKey = Configuration[ConfigurationNameConstants.SEQ_API_KEY];
                Log.Logger = new LoggerConfiguration()
                                .WriteTo.Seq(seqUrl, apiKey: seqApiKey)
                                .WriteTo.Console().CreateLogger();
            }
            else
            {
                var currentDirectory = Directory.GetCurrentDirectory();
                var logPath = Path.Combine(currentDirectory, EnvironmentConstants.LOG_FILE_FOLDER, EnvironmentConstants.LOG_FILE_NAME);
                Log.Logger = new LoggerConfiguration()
                                .WriteTo.File(path: logPath, rollingInterval: RollingInterval.Day)
                                .WriteTo.Console().CreateLogger();
            }
        }

        private async Task PreparateDatabase(WebApplication app)
        {
            var persistanceMode = Configuration.GetValue<PersistanceMode>(nameof(PersistanceMode));
            if (persistanceMode == PersistanceMode.Database)
            {
                var connectionTimeOut = GetConnectionTimeOut();
                var preparationDatabase = app.Services.GetService<PreparationDatabase>();
                var task = preparationDatabase!.PreparatePopulation(app, connectionTimeOut);
                await task;
            }
        }

        private void SetConnectionString()
        {
            SetConnectionStringFromConfig();
            SetDebugConnectionString();
        }

        [Conditional("DEBUG")]
        private void SetDebugConnectionString()
        {
            _connectionString = DefaultValueConstants.DEBUG_CONNECTION_STRING;
        }

        [Conditional("RELEASE")]
        private void SetConnectionStringFromConfig()
        {
            var databaseName = Configuration[ConfigurationNameConstants.DB_NAME];
            var password = Configuration[ConfigurationNameConstants.DB_PASSWORD];
            var port = Configuration[ConfigurationNameConstants.DB_PORT];
            var server = Configuration[ConfigurationNameConstants.DB_SERVER];
            var user = Configuration[ConfigurationNameConstants.DB_USER];

            // INFO: https://github.com/dotnet/SqlClient/issues/1479
            _connectionString = $"Server={server},{port};Database={databaseName};Trusted_Connection=False;User Id={user};Password={password};TrustServerCertificate=True";
        }

        private int GetConnectionTimeOut()
        {
            var connectionTimeOutInSecondsStringValue = Configuration[ConfigurationNameConstants.DB_CONNECTION_TIME_OUT_IN_SECONDS];
            int timeOutInSeconds = int.TryParse(connectionTimeOutInSecondsStringValue, out timeOutInSeconds) ? timeOutInSeconds : DefaultValueConstants.DEFAULT_DB_MIGRATION_CONNECTION_TIME_OUT_IN_SECONDS;
            var connectionTimeOut = timeOutInSeconds.ToMilliseconds();

            return connectionTimeOut;
        }
    }
}
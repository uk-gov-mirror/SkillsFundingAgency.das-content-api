using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using SFA.DAS.AutoConfiguration;

namespace SFA.DAS.ContentApi.Data
{
    public class DbContextWithNewTransactionFactory : IContentApiDbContextFactory
    {
        private readonly SqlConnection _sqlConnection;
        private readonly IEnvironmentService _environmentService;
        private readonly ILoggerFactory _loggerFactory;

        public DbContextWithNewTransactionFactory(SqlConnection sqlConnection, IEnvironmentService environmentService, ILoggerFactory loggerFactory)
        {
            _sqlConnection = sqlConnection;
            _environmentService = environmentService;
            _loggerFactory = loggerFactory;
        }

        public ContentApiDbContext CreateDbContext()
        {
            if (!_environmentService.IsCurrent(DasEnv.LOCAL))
            {
                AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
                _sqlConnection.AccessToken = azureServiceTokenProvider.GetAccessTokenAsync("https://database.windows.net/").GetAwaiter().GetResult();
            }

            var optionsBuilder = new DbContextOptionsBuilder<ContentApiDbContext>()
                .UseSqlServer(_sqlConnection)
                .ConfigureWarnings(w => w.Throw(RelationalEventId.QueryClientEvaluationWarning));

            if (_environmentService.IsCurrent(DasEnv.LOCAL))
            {
                optionsBuilder.UseLoggerFactory(_loggerFactory);
            }

            var dbContext = new ContentApiDbContext(optionsBuilder.Options);

            return dbContext;
        }
    }
}
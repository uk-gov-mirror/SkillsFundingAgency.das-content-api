using System.Data.Common;
using System.Data.SqlClient;
using SFA.DAS.ContentApi.Configuration;
using SFA.DAS.ContentApi.Data;
using StructureMap;

namespace SFA.DAS.ContentApi.DependencyResolution
{
    public class DataRegistry : Registry
    {
        public DataRegistry()
        {
            For<SqlConnection>().Use(c => new SqlConnection(c.GetInstance<ContentApiSettings>().DatabaseConnectionString));
            For<ContentApiDbContext>().Use(c => c.GetInstance<IContentApiDbContextFactory>().CreateDbContext());
        }
    }
}
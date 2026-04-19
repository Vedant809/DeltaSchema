using DeltaSchema.DTOs;
using DeltaSchema.Interface;

namespace DeltaSchema.Manager
{
    public class ConnectionStringResolver:IConnectionStringResolver
    {
        public string GetConnectionString(ConnectionDetails request)
        {
            var connectionString = $"Server={request.Host};Database={request.Database};Trusted_Connection=True;TrustServerCertificate=True;";
            return connectionString;
        }
    }
}

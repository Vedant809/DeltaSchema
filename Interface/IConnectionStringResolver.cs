using DeltaSchema.DTOs;

namespace DeltaSchema.Interface
{
    public interface IConnectionStringResolver
    {
        string GetConnectionString(ConnectionDetails request);
    }
}

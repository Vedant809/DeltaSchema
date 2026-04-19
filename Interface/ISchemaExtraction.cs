using DeltaSchema.DTOs;

namespace DeltaSchema.Interface
{
    public interface ISchemaExtraction
    {
        List<Schema> ExtractTableMetadata(ConnectionDetails request);
    }
}

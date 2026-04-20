using DeltaSchema.DTOs;

namespace DeltaSchema.Interface
{
    public interface ISchemaExtraction
    {
        //
        SchemaDiff ExtractTableMetadata(CombinedConnectionDetails request);
    }
}

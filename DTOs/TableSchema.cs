namespace DeltaSchema.DTOs
{
    public class TableSchema
    {
        //public string? TableName { get; set; }
        ////public List<Columns>? Columns { get; set; }
        //public string? Schema { get; set; }

        public string TABLE_SCHEMA { get; set; }
        public string TABLE_NAME { get; set; }

    }

    public class Schema
    {
        public string? TABLE_NAME { get; set; }
        public string? COLUMN_NAME { get; set; }
        public string? DATA_TYPE { get; set; }
        public string? IS_NULLABLE { get; set; }
        public int? CHARACTER_MAXIMUM_LENGTH { get; set; }
    }

    public class TableInfo
    {
        public string TABLE_SCHEMA { get; set; }
        public string TABLE_NAME { get; set; }
    }
    public class Columns
    {
        public string? Name { get; set; }
        public string? DataType { get; set; }
    }
    public class ConnectionDetails
    {
        public string? Host { get; set; }
        public string? Database { get; set; }
        public string? Password { get; set; }
    }
}

using System.Collections.Immutable;
using DeltaSchema.DTOs;
using DeltaSchema.Interface;
using DeltaSchema.Repository;
using Microsoft.EntityFrameworkCore;

namespace DeltaSchema.Manager
{
    public class SchemaExtraction:ISchemaExtraction
    {
        private readonly IConnectionStringResolver _resolver;
        public SchemaExtraction(IConnectionStringResolver resolver)
        {
            _resolver = resolver;
        }
        public List<Schema> ExtractTableMetadata(ConnectionDetails request)
        {
            var conn = _resolver.GetConnectionString(request);
            var options = new DbContextOptionsBuilder<API_DbContext>()
                            .UseSqlServer(conn)
                            .Options;

            using var _context = new API_DbContext(options);
            //var tables = _context.Database.SqlQueryRaw<TableSchema>(@"
            //                        SELECT TABLE_SCHEMA, TABLE_NAME
            //                        FROM INFORMATION_SCHEMA.TABLES
            //                        WHERE TABLE_TYPE = 'BASE TABLE'").
            //                        ToList();
            //Extract the columns of all the tables

            var columns = _context.Database.SqlQueryRaw<Schema>(@"
                    SELECT TABLE_NAME,COLUMN_NAME, DATA_TYPE, IS_NULLABLE,CHARACTER_MAXIMUM_LENGTH
                    FROM INFORMATION_SCHEMA.COLUMNS
                    Where TABLE_NAME in (SELECT TABLE_NAME
                                    FROM INFORMATION_SCHEMA.TABLES
                                    WHERE TABLE_TYPE = 'BASE TABLE')
            ")
             .ToList();

            //var dict = columns
            //    .GroupBy(x => new { x.TABLE_NAME, x })
            //    .ToList();
            //    //.ToDictionary()

            //    //.Select(x => new
            //    //{
            //    //    x.Key.TABLE_NAME,
            //    //    x
            //    //})
            //    //.ToList<dynamic>();


            var dict = columns
                .GroupBy(x => new { x.TABLE_NAME, x.COLUMN_NAME, x.DATA_TYPE, x.IS_NULLABLE, x.CHARACTER_MAXIMUM_LENGTH })
                .Select(y => new Schema()
                {
                    TABLE_NAME = y.Key.TABLE_NAME,
                    COLUMN_NAME = y.Key.COLUMN_NAME,
                    DATA_TYPE = y.Key.DATA_TYPE,
                    IS_NULLABLE = y.Key.IS_NULLABLE,
                    CHARACTER_MAXIMUM_LENGTH = y.Key.CHARACTER_MAXIMUM_LENGTH
                })
                .ToList();


            return dict;
        }
    }
}

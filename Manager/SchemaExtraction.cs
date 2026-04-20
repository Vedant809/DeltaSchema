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
        public SchemaDiff ExtractTableMetadata(CombinedConnectionDetails request)
        {
            //Source Table

            var conn = _resolver.GetConnectionString(request.Source);
            var options = new DbContextOptionsBuilder<API_DbContext>()
                            .UseSqlServer(conn)
                            .Options;

            using var _context = new API_DbContext(options);
            //Extract the columns of all the tables

            var columns = _context.Database.SqlQueryRaw<Schema>(@"
                    SELECT TABLE_NAME,COLUMN_NAME, DATA_TYPE, IS_NULLABLE,CHARACTER_MAXIMUM_LENGTH
                    FROM INFORMATION_SCHEMA.COLUMNS
                    Where TABLE_NAME in (SELECT TABLE_NAME
                                    FROM INFORMATION_SCHEMA.TABLES
                                    WHERE TABLE_TYPE = 'BASE TABLE')
            ")
             .ToList();

            //Target Table

            var conn1 = _resolver.GetConnectionString(request.Target);
            var options1 = new DbContextOptionsBuilder<API_DbContext>()
                            .UseSqlServer(conn1)
                            .Options;

            using var _context1 = new API_DbContext(options1);
            //Extract the columns of all the tables

            var columns1 = _context1.Database.SqlQueryRaw<Schema>(@"
                    SELECT TABLE_NAME,COLUMN_NAME, DATA_TYPE, IS_NULLABLE,CHARACTER_MAXIMUM_LENGTH
                    FROM INFORMATION_SCHEMA.COLUMNS
                    Where TABLE_NAME in (SELECT TABLE_NAME
                                    FROM INFORMATION_SCHEMA.TABLES
                                    WHERE TABLE_TYPE = 'BASE TABLE')
            ")
             .ToList();

            var targetDict = columns1
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


            var sourceDict = columns
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
            //Table missing ---> Only in source and not in target == CREAT TABLE

            var tableMissing = sourceDict.ExceptBy(targetDict.Select(x => x.TABLE_NAME), x => x.TABLE_NAME).ToList();


            //Columns In source but not in target -- ALTER TABLE

            var schemaDiff = sourceDict.ExceptBy(targetDict.Select(x=>x.COLUMN_NAME),x=>x.COLUMN_NAME).ToList();

            // Datatype mismatch for known columns -- Alter

            var columnDiff = sourceDict.ExceptBy(targetDict.Select(x => x.CHARACTER_MAXIMUM_LENGTH), x => x.CHARACTER_MAXIMUM_LENGTH).ToList();

            var schemaDiff1 = schemaDiff.UnionBy(columnDiff, x => x.TABLE_NAME).ToList();

            List<Schema> dtypeMismatch = new List<Schema>();

            foreach (var item in targetDict)
            {
                var dtypeMismatch1 = sourceDict.Where(x => x.TABLE_NAME == item.TABLE_NAME &&
                x.COLUMN_NAME == item.COLUMN_NAME &&
                x.CHARACTER_MAXIMUM_LENGTH != item.CHARACTER_MAXIMUM_LENGTH)
                    .ToList();
                dtypeMismatch.AddRange(dtypeMismatch1);
            }



            return new SchemaDiff()
            {
                TableMissing = tableMissing,
                ColumnMissing = schemaDiff1,
                DataTypeMismatch = dtypeMismatch
            };
        }
    }
}

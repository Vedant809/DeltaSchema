using DeltaSchema.Interface;
using Microsoft.EntityFrameworkCore;

namespace DeltaSchema.Repository
{
    public class API_DbContext:DbContext
    {
        public API_DbContext(DbContextOptions<API_DbContext> options) : base(options)
        {
        }

    }
}

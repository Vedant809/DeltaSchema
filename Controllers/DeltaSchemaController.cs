using DeltaSchema.DTOs;
using DeltaSchema.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeltaSchema.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeltaSchemaController : ControllerBase
    {
        private readonly ISchemaExtraction _service;
        public DeltaSchemaController(ISchemaExtraction service)
        {
            _service = service;
        }
        [HttpPost("ExtractTableMetadata")]
        public SchemaDiff ExtractTableMetadata(CombinedConnectionDetails request)
        {
            return _service.ExtractTableMetadata(request);
        }
    }
}

using ControlX.Insights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ControlX.Hub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InsightsController : ControllerBase
    {
        private readonly LogService _logService;

        public InsightsController(LogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var result = await _logService.GetTraces();

            return Ok(result);
        }
    }
}

using GanPersonWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace GanPersonWeb.Controllers
{
    [ApiController]
    [Route("api/visit")]
    public class SiteVisitController : ControllerBase
    {
        private readonly SiteVisitService _siteVisitService;
        public SiteVisitController(SiteVisitService siteVisitService)
        {
            _siteVisitService = siteVisitService;
        }

        [HttpPost("record")]
        public async Task<IActionResult> RecordVisit()
        {
            await _siteVisitService.RecordVisitAsync();
            return Ok();
        }

        [HttpGet("data")]
        public async Task<IActionResult> GetVisitdata()
        {
            var visits = await _siteVisitService.GetVisitsAsync();
            return Ok(visits);
        }
    }

}

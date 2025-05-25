using GanPersonWeb.Services;
using GanPersonWeb.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GanPersonWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonalInfoController : ControllerBase
    {
        private readonly PersonalInfoService _personalInfoService;

        public PersonalInfoController(PersonalInfoService personalInfoService)
        {
            _personalInfoService = personalInfoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPersonalInfo()
        {
            var personalInfo = await _personalInfoService.GetPersonalInfoAsync();
            if (personalInfo == null)
                return NotFound();

            return Ok(personalInfo);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdatePersonalInfo([FromBody] PersonalInfo personalInfo)
        {
            await _personalInfoService.UpdatePersonalInfoAsync(personalInfo);
            return NoContent();
        }
    }
}

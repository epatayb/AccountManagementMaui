using Microsoft.AspNetCore.Mvc;

namespace AccountManagementMaui.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Success = true,
                Message = "AccountManagement API çalışıyor.",
                ServerTime = DateTime.UtcNow
            });
        }
    }
}
namespace BulkRename.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class HealthzController : ControllerBase
    {
        private readonly ILogger<HealthzController> _logger;

        public HealthzController(ILogger<HealthzController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("HealtchCheck running...");
            return Ok("Container is healthy");
        }
    }
}

using BusinessLogic.Services.Core.Base;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/seed-data")]
    public class DataSeedingController : ControllerBase
    {
        private readonly IDataSeedingService _dataSeedingService;

        public DataSeedingController(IDataSeedingService dataSeedingService)
        {
            _dataSeedingService = dataSeedingService;
        }

        [HttpPost]
        public async Task<IActionResult> SeedAsync(CancellationToken cancellationToken)
        {
            var result = await _dataSeedingService.SeedAsync(cancellationToken: cancellationToken);

            if (result)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}

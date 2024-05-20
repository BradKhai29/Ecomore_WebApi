using DataAccess.DbContexts;
using DataAccess.UnitOfWorks.Base;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/health-check")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;

        public HealthCheckController(IUnitOfWork<AppDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> CheckHealthAsync()
        {
            await _unitOfWork.HealthCheckRepository.CheckHealthAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UploadAsync(
            IFormFile formFile)
        {
            try
            {
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}

using Devices_tasks1_2_3.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Devices_tasks1_2_3.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsApiController : ControllerBase
    {
        private readonly ReportRepository _reportRepository;

        public ReportsApiController(
            ReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        [HttpGet("GetClientsPerType")]
        public IActionResult GetClientsPerType(
            [FromQuery] int? type)
        {
            var result =
                _reportRepository.GetClientsPerType(type);

            return Ok(result);
        }

        [HttpGet("GetPhoneNumbersPerDevice")]
        public IActionResult GetPhoneNumbersPerDevice(
    [FromQuery] int? deviceId,
    [FromQuery] int? status)
        {
            var result =
                _reportRepository.GetPhoneNumbersPerDevice(
                    deviceId,
                    status
                );

            return Ok(result);
        }
    }
}
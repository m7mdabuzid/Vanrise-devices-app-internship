using System;
using Devices_tasks1_2_3.Data;
using Devices_tasks1_2_3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Devices_tasks1_2_3.Controllers.Api
{
    [ApiController]
    [Route("api/devices")]
    public class DevicesApiController : ControllerBase
    {
        private readonly DeviceRepository _deviceRepository;

        public DevicesApiController(
            DeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }

        [HttpGet("GetAllDevices")]
        public IActionResult GetAllDevices()
        {
            try
            {
                var devices =
                    _deviceRepository.GetAllDevices();

                return Ok(devices);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message =
                        "Devices could not be loaded. " +
                        ex.Message
                });
            }
        }

        [HttpGet("GetFilteredDevices")]
        public IActionResult GetFilteredDevices(
            [FromQuery] string? searchValue)
        {
            try
            {
                var devices =
                    _deviceRepository
                        .GetFilteredDevices(searchValue);

                return Ok(devices);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message =
                        "Devices could not be filtered. " +
                        ex.Message
                });
            }
        }

        [HttpPost("AddDevice")]
        public IActionResult AddDevice(
            [FromBody] Device device)
        {
            try
            {
                _deviceRepository.AddDevice(device);

                return Ok(new
                {
                    message =
                        "Device added successfully."
                });
            }
            catch (SqlException ex)
                when (ex.Number == 2601 ||
                      ex.Number == 2627)
            {
                return BadRequest(new
                {
                    message =
                        "A device with this name already exists."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message =
                        "Device could not be added. " +
                        ex.Message
                });
            }
        }

        [HttpPut("UpdateDevice")]
        public IActionResult UpdateDevice(
            [FromBody] Device device)
        {
            try
            {
                _deviceRepository.UpdateDevice(device);

                return Ok(new
                {
                    message =
                        "Device updated successfully."
                });
            }
            catch (SqlException ex)
                when (ex.Number == 2601 ||
                      ex.Number == 2627)
            {
                return BadRequest(new
                {
                    message =
                        "A device with this name already exists."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message =
                        "Device could not be updated. " +
                        ex.Message
                });
            }
        }

        [HttpDelete("DeleteDevice/{id:int}")]
        public IActionResult DeleteDevice(int id)
        {
            try
            {
                _deviceRepository.DeleteDevice(id);

                return Ok(new
                {
                    message =
                        "Device deleted successfully."
                });
            }
            catch (SqlException ex)
                when (ex.Number == 547)
            {
                return BadRequest(new
                {
                    message =
                        "The device cannot be deleted because phone numbers are linked to it."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message =
                        "Device could not be deleted. " +
                        ex.Message
                });
            }
        }
    }
}
using System;
using Devices_tasks1_2_3.Data;
using Devices_tasks1_2_3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Devices_tasks1_2_3.Controllers.Api
{
    [ApiController]
    [Route("api/phonenumbers")]
    public class PhoneNumbersApiController : ControllerBase
    {
        private readonly PhoneNumberRepository
            _phoneNumberRepository;

        public PhoneNumbersApiController(
            PhoneNumberRepository phoneNumberRepository)
        {
            _phoneNumberRepository =
                phoneNumberRepository;
        }

        [HttpGet("GetAllPhoneNumbers")]
        public IActionResult GetAllPhoneNumbers()
        {
            try
            {
                var phoneNumbers =
                    _phoneNumberRepository
                        .GetAllPhoneNumbers();

                return Ok(phoneNumbers);
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    message =
                        "Phone numbers could not be loaded."
                });
            }
        }

        [HttpGet("GetFilteredPhoneNumbers")]
        public IActionResult GetFilteredPhoneNumbers(
            [FromQuery] string? number,
            [FromQuery] int? deviceId)
        {
            try
            {
                var phoneNumbers =
                    _phoneNumberRepository
                        .GetFilteredPhoneNumbers(
                            number,
                            deviceId);

                return Ok(phoneNumbers);
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    message =
                        "Phone numbers could not be filtered."
                });
            }
        }

        [HttpPost("AddPhoneNumber")]
        public IActionResult AddPhoneNumber(
            [FromBody] PhoneNumber phoneNumber)
        {
            try
            {
                _phoneNumberRepository
                    .AddPhoneNumber(phoneNumber);

                return Ok(new
                {
                    message =
                        "Phone number added successfully."
                });
            }
            catch (SqlException ex)
                when (
                    ex.Number == 50001 ||
                    ex.Number == 2601 ||
                    ex.Number == 2627
                )
            {
                return BadRequest(new
                {
                    message =
                        "This phone number already exists."
                });
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    message =
                        "Phone number could not be added."
                });
            }
        }

        [HttpPut("UpdatePhoneNumber")]
        public IActionResult UpdatePhoneNumber(
            [FromBody] PhoneNumber phoneNumber)
        {
            try
            {
                bool updated =
                    _phoneNumberRepository
                        .UpdatePhoneNumber(phoneNumber);

                if (!updated)
                {
                    return NotFound(new
                    {
                        message =
                            "The phone number no longer exists."
                    });
                }

                return Ok(new
                {
                    message =
                        "Phone number updated successfully."
                });
            }
            catch (SqlException ex)
                when (
                    ex.Number == 50001 ||
                    ex.Number == 2601 ||
                    ex.Number == 2627
                )
            {
                return BadRequest(new
                {
                    message =
                        "This phone number already exists."
                });
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    message =
                        "Phone number could not be updated."
                });
            }
        }

        [HttpDelete("DeletePhoneNumber/{id:int}")]
        public IActionResult DeletePhoneNumber(int id)
        {
            try
            {
                bool deleted =
                    _phoneNumberRepository
                        .DeletePhoneNumber(id);

                if (!deleted)
                {
                    return NotFound(new
                    {
                        message =
                            "The phone number no longer exists."
                    });
                }

                return Ok(new
                {
                    message =
                        "Phone number deleted successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (SqlException ex)
                when (ex.Number == 547)
            {
                return BadRequest(new
                {
                    message =
                        "The phone number cannot be deleted because related records still exist."
                });
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    message =
                        "Phone number could not be deleted."
                });
            }
        }
    }
}
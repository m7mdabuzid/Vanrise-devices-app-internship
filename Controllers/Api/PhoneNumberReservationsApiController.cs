using System;
using Devices_tasks1_2_3.Data;
using Devices_tasks1_2_3.Models;
using Microsoft.AspNetCore.Mvc;

namespace Devices_tasks1_2_3.Controllers.Api
{
    [ApiController]
    [Route("api/phonenumberreservations")]
    public class PhoneNumberReservationsApiController : ControllerBase
    {
        private readonly PhoneNumberReservationRepository
            _phoneNumberReservationRepository;

        public PhoneNumberReservationsApiController(
            PhoneNumberReservationRepository
                phoneNumberReservationRepository)
        {
            _phoneNumberReservationRepository =
                phoneNumberReservationRepository;
        }


        [HttpGet("GetAllPhoneNumberReservations")]
        public IActionResult GetAllPhoneNumberReservations()
        {
            var reservations =
                _phoneNumberReservationRepository
                    .GetAllReservations();

            return Ok(reservations);
        }


        [HttpGet("GetFilteredPhoneNumberReservations")]
        public IActionResult GetFilteredPhoneNumberReservations(
            [FromQuery] int? clientId,
            [FromQuery] int? phoneNumberId)
        {
            var reservations =
                _phoneNumberReservationRepository
                    .GetFilteredReservations(
                        clientId,
                        phoneNumberId);

            return Ok(reservations);
        }


        [HttpGet("GetAvailablePhoneNumbers")]
        public IActionResult GetAvailablePhoneNumbers()
        {
            var phoneNumbers =
                _phoneNumberReservationRepository
                    .GetAvailablePhoneNumbers();

            return Ok(phoneNumbers);
        }


        [HttpGet(
            "GetActivePhoneNumbersByClient/{clientId:int}")]
        public IActionResult GetActivePhoneNumbersByClient(
            int clientId)
        {
            var phoneNumbers =
                _phoneNumberReservationRepository
                    .GetActivePhoneNumbersByClient(clientId);

            return Ok(phoneNumbers);
        }


        [HttpPost("ReservePhoneNumber")]
        public IActionResult ReservePhoneNumber(
            [FromBody]
            PhoneNumberReservationActionRequest request)
        {
            try
            {
                _phoneNumberReservationRepository
                    .ReservePhoneNumber(
                        request.ClientId,
                        request.PhoneNumberId);

                return Ok(new
                {
                    message =
                        "Phone number reserved successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message =
                        "Phone number could not be reserved. " +
                        ex.Message
                });
            }
        }


        [HttpPut("UnreservePhoneNumber")]
        public IActionResult UnreservePhoneNumber(
            [FromBody]
            PhoneNumberReservationActionRequest request)
        {
            try
            {
                _phoneNumberReservationRepository
                    .UnreservePhoneNumber(
                        request.ClientId,
                        request.PhoneNumberId);

                return Ok(new
                {
                    message =
                        "Phone number unreserved successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message =
                        "Phone number could not be unreserved. " +
                        ex.Message
                });
            }
        }
    }
}
using System;
using Devices_tasks1_2_3.Data;
using Devices_tasks1_2_3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Devices_tasks1_2_3.Controllers.Api
{
    [ApiController]
    [Route("api/clients")]
    public class ClientsApiController : ControllerBase
    {
        private readonly ClientRepository _clientRepository;

        public ClientsApiController(
            ClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpGet("GetAllClients")]
        public IActionResult GetAllClients()
        {
            try
            {
                return Ok(
                    _clientRepository.GetAllClients()
                );
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    message = "Clients could not be loaded."
                });
            }
        }

        [HttpGet("GetFilteredClients")]
        public IActionResult GetFilteredClients(
            [FromQuery] string? name,
            [FromQuery] int? type)
        {
            try
            {
                return Ok(
                    _clientRepository.GetFilteredClients(
                        name,
                        type
                    )
                );
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    message = "Clients could not be filtered."
                });
            }
        }

        [HttpPost("AddClient")]
        public IActionResult AddClient(
            [FromBody] Client client)
        {
            try
            {
                _clientRepository.AddClient(client);

                return Ok(new
                {
                    message = "Client added successfully."
                });
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
    when (ex.Number == 50002 ||
          ex.Number == 2601 ||
          ex.Number == 2627)
            {
                return BadRequest(new
                {
                    message =
                        "An organization with this name already exists."
                });
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    message = "Client could not be added."
                });
            }
        }

        [HttpPut("UpdateClient")]
        public IActionResult UpdateClient(
            [FromBody] Client client)
        {
            try
            {
                bool updated =
                    _clientRepository.UpdateClient(client);

                if (!updated)
                {
                    return NotFound(new
                    {
                        message =
                            "The client no longer exists."
                    });
                }

                return Ok(new
                {
                    message = "Client updated successfully."
                });
            }

            catch (Microsoft.Data.SqlClient.SqlException ex)
    when (ex.Number == 50002 ||
          ex.Number == 2601 ||
          ex.Number == 2627)
            {
                return BadRequest(new
                {
                    message =
                        "An organization with this name already exists."
                });
            }

            catch (Exception)
            {
                return BadRequest(new
                {
                    message = "Client could not be updated."
                });
            }
        }

        [HttpDelete("DeleteClient/{id:int}")]
        public IActionResult DeleteClient(int id)
        {
            try
            {
                bool deleted =
                    _clientRepository.DeleteClient(id);

                if (!deleted)
                {
                    return NotFound(new
                    {
                        message =
                            "The client no longer exists."
                    });
                }

                return Ok(new
                {
                    message = "Client deleted successfully."
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
                        "The client cannot be deleted because related records still exist."
                });
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    message = "Client could not be deleted."
                });
            }
        }
    }
}
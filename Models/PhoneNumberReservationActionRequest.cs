using System.ComponentModel.DataAnnotations;

namespace Devices_tasks1_2_3.Models
{
    public class PhoneNumberReservationActionRequest
    {
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "A client must be selected.")]
        public int ClientId { get; set; }

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "A phone number must be selected.")]
        public int PhoneNumberId { get; set; }
    }
}
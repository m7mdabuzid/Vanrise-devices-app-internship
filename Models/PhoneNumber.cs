using System.ComponentModel.DataAnnotations;

namespace Devices_tasks1_2_3.Models
{
    public class PhoneNumber
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(
            @"^\d{8}$",
            ErrorMessage = "Phone number must contain exactly 8 digits.")]
        public string Number { get; set; } = string.Empty;

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "A device must be selected.")]
        public int DeviceId { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace Devices_tasks1_2_3.Models
{
    public class Device
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Device name is required.")]
        [StringLength(
            100,
            MinimumLength = 2,
            ErrorMessage = "Device name must contain between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;
    }
}
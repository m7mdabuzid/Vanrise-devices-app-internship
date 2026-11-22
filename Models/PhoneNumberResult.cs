namespace Devices_tasks1_2_3.Models
{
    public class PhoneNumberResult
    {
        public int Id { get; set; }

        public string Number { get; set; } = string.Empty;

        public int DeviceId { get; set; }

        public string DeviceName { get; set; } = string.Empty;
    }
}
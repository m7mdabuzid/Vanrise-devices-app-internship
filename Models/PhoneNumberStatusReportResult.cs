namespace Devices_tasks1_2_3.Models
{
    public class PhoneNumberStatusReportResult
    {
        public int DeviceId { get; set; }

        public string DeviceName { get; set; } = string.Empty;

        public int Status { get; set; }

        public string StatusName { get; set; } = string.Empty;

        public int NumberOfPhoneNumbers { get; set; }
    }
}
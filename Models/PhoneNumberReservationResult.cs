namespace Devices_tasks1_2_3.Models
{
    public class PhoneNumberReservationResult
    {
        public int Id { get; set; }

        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;

        public int PhoneNumberId { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
    }
}
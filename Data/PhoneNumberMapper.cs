using Devices_tasks1_2_3.Models;
using Microsoft.Data.SqlClient;

namespace Devices_tasks1_2_3.Data
{
    public static class PhoneNumberMapper
    {
        public static PhoneNumberResult MapPhoneNumber(
            SqlDataReader reader)
        {
            return new PhoneNumberResult
            {
                Id = reader.GetInt32(
                    reader.GetOrdinal("Id")),

                Number = reader.GetString(
                    reader.GetOrdinal("Number")),

                DeviceId = reader.GetInt32(
                    reader.GetOrdinal("DeviceId")),

                DeviceName = reader.GetString(
                    reader.GetOrdinal("DeviceName"))
            };
        }

        public static List<PhoneNumberResult> MapPhoneNumbers(
            SqlDataReader reader)
        {
            var phoneNumbers =
                new List<PhoneNumberResult>();

            while (reader.Read())
            {
                phoneNumbers.Add(MapPhoneNumber(reader));
            }

            return phoneNumbers;
        }
    }
}
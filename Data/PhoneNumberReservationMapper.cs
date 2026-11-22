using Devices_tasks1_2_3.Models;
using Microsoft.Data.SqlClient;

namespace Devices_tasks1_2_3.Data
{
    public static class PhoneNumberReservationMapper
    {
        public static PhoneNumberReservationResult MapReservation(
            SqlDataReader reader)
        {
            int eedPosition = reader.GetOrdinal("EED");

            return new PhoneNumberReservationResult
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                ClientId = reader.GetInt32(reader.GetOrdinal("ClientId")),
                ClientName = reader.GetString(reader.GetOrdinal("ClientName")),
                PhoneNumberId = reader.GetInt32(reader.GetOrdinal("PhoneNumberId")),
                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                BED = reader.GetDateTime(reader.GetOrdinal("BED")),
                EED = reader.IsDBNull(eedPosition)
                    ? null
                    : reader.GetDateTime(eedPosition)
            };
        }

        public static List<PhoneNumberReservationResult> MapReservations(
            SqlDataReader reader)
        {
            var reservations =
                new List<PhoneNumberReservationResult>();

            while (reader.Read())
            {
                reservations.Add(MapReservation(reader));
            }

            return reservations;
        }
    }
}
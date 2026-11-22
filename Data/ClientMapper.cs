using Devices_tasks1_2_3.Models;
using Microsoft.Data.SqlClient;

namespace Devices_tasks1_2_3.Data
{
    public static class ClientMapper
    {
        public static Client MapClient(SqlDataReader reader)
        {
            return new Client
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Type = (ClientType)reader.GetInt32(reader.GetOrdinal("Type")),
                BirthDate = reader.IsDBNull(reader.GetOrdinal("BirthDate"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("BirthDate")),
                HasActiveReservation =
    reader.GetBoolean(
        reader.GetOrdinal("HasActiveReservation"))
            };
        }

        public static List<Client> MapClients(SqlDataReader reader)
        {
            var clients = new List<Client>();

            while (reader.Read())
            {
                clients.Add(MapClient(reader));
            }

            return clients;
        }
    }
}
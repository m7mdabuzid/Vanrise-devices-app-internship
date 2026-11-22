using Devices_tasks1_2_3.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Devices_tasks1_2_3.Data
{
    public class PhoneNumberReservationRepository
    {
        private readonly string _connectionString;

        public PhoneNumberReservationRepository(
            IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DevicesConnection")
                ?? throw new InvalidOperationException(
                    "DevicesConnection was not found.");
        }

        public List<PhoneNumberReservationResult>
            GetAllReservations()
        {
            const string query = @"
                SELECT
                    r.Id,
                    r.ClientId,
                    c.Name AS ClientName,
                    r.PhoneNumberId,
                    p.Number AS PhoneNumber,
                    r.BED,
                    r.EED
                FROM dbo.PhoneNumberReservations r
                INNER JOIN dbo.Clients c
                    ON r.ClientId = c.Id
                INNER JOIN dbo.PhoneNumbers p
                    ON r.PhoneNumberId = p.Id
                ORDER BY r.Id;";

            using SqlConnection connection =
                new(_connectionString);

            using SqlCommand command =
                new(query, connection);

            connection.Open();

            using SqlDataReader reader =
                command.ExecuteReader();

            return PhoneNumberReservationMapper
                .MapReservations(reader);
        }

        public List<PhoneNumberReservationResult>
            GetFilteredReservations(
                int? clientId,
                int? phoneNumberId)
        {
            const string query = @"
                SELECT
                    r.Id,
                    r.ClientId,
                    c.Name AS ClientName,
                    r.PhoneNumberId,
                    p.Number AS PhoneNumber,
                    r.BED,
                    r.EED
                FROM dbo.PhoneNumberReservations r
                INNER JOIN dbo.Clients c
                    ON r.ClientId = c.Id
                INNER JOIN dbo.PhoneNumbers p
                    ON r.PhoneNumberId = p.Id
                WHERE
                    (@ClientId IS NULL
                     OR r.ClientId = @ClientId)
                    AND
                    (@PhoneNumberId IS NULL
                     OR r.PhoneNumberId = @PhoneNumberId)
                ORDER BY r.Id;";

            using SqlConnection connection =
                new(_connectionString);

            using SqlCommand command =
                new(query, connection);

            command.Parameters.Add(
                "@ClientId",
                SqlDbType.Int
            ).Value = clientId.HasValue
                ? clientId.Value
                : DBNull.Value;

            command.Parameters.Add(
                "@PhoneNumberId",
                SqlDbType.Int
            ).Value = phoneNumberId.HasValue
                ? phoneNumberId.Value
                : DBNull.Value;

            connection.Open();

            using SqlDataReader reader =
                command.ExecuteReader();

            return PhoneNumberReservationMapper
                .MapReservations(reader);
        }
        public List<PhoneNumberResult> GetAvailablePhoneNumbers()
        {
            const string query = @"
        SELECT
            p.Id,
            p.Number,
            p.DeviceId,
            d.Name AS DeviceName
        FROM dbo.PhoneNumbers p
        INNER JOIN dbo.Devices d
            ON p.DeviceId = d.Id
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM dbo.PhoneNumberReservations r
            WHERE r.PhoneNumberId = p.Id
              AND GETDATE() > r.BED
              AND (r.EED IS NULL OR GETDATE() < r.EED)
        )
        ORDER BY p.Number;";

            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = new(query, connection);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            return PhoneNumberMapper.MapPhoneNumbers(reader);
        }

        public List<PhoneNumberResult> GetActivePhoneNumbersByClient(
            int clientId)
        {
            const string query = @"
        SELECT
            p.Id,
            p.Number,
            p.DeviceId,
            d.Name AS DeviceName
        FROM dbo.PhoneNumberReservations r
        INNER JOIN dbo.PhoneNumbers p
            ON r.PhoneNumberId = p.Id
        INNER JOIN dbo.Devices d
            ON p.DeviceId = d.Id
        WHERE r.ClientId = @ClientId
          AND GETDATE() > r.BED
          AND (r.EED IS NULL OR GETDATE() < r.EED)
        ORDER BY p.Number;";

            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = new(query, connection);

            command.Parameters.Add(
                "@ClientId",
                SqlDbType.Int).Value = clientId;

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            return PhoneNumberMapper.MapPhoneNumbers(reader);
        }

        public bool ReservePhoneNumber(
            int clientId,
            int phoneNumberId)
        {
            const string query = @"
        INSERT INTO dbo.PhoneNumberReservations
            (ClientId, PhoneNumberId, BED, EED)
        SELECT
            @ClientId,
            @PhoneNumberId,
            @Now,
            NULL
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM dbo.PhoneNumberReservations
            WHERE PhoneNumberId = @PhoneNumberId
              AND @Now > BED
              AND (EED IS NULL OR @Now < EED)
        );";

            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = new(query, connection);

            command.Parameters.Add("@ClientId", SqlDbType.Int)
                .Value = clientId;

            command.Parameters.Add("@PhoneNumberId", SqlDbType.Int)
                .Value = phoneNumberId;

            command.Parameters.Add("@Now", SqlDbType.DateTime2)
                .Value = DateTime.Now;

            connection.Open();

            return command.ExecuteNonQuery() > 0;
        }

        public bool UnreservePhoneNumber(
            int clientId,
            int phoneNumberId)
        {
            const string query = @"
        UPDATE dbo.PhoneNumberReservations
        SET EED = @Now
        WHERE ClientId = @ClientId
          AND PhoneNumberId = @PhoneNumberId
          AND @Now > BED
          AND (EED IS NULL OR @Now < EED);";

            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = new(query, connection);

            command.Parameters.Add("@ClientId", SqlDbType.Int)
                .Value = clientId;

            command.Parameters.Add("@PhoneNumberId", SqlDbType.Int)
                .Value = phoneNumberId;

            command.Parameters.Add("@Now", SqlDbType.DateTime2)
                .Value = DateTime.Now;

            connection.Open();

            return command.ExecuteNonQuery() > 0;
        }
    }
}
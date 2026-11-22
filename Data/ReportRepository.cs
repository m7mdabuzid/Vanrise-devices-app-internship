using Devices_tasks1_2_3.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Devices_tasks1_2_3.Repositories
{
    public class ReportRepository
    {
        private readonly string _connectionString;

        public ReportRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DevicesConnection")
                ?? throw new InvalidOperationException(
                    "DevicesConnection was not found.");
        }

        public List<ClientTypeReportResult> GetClientsPerType(
            int? type)
        {
            var results = new List<ClientTypeReportResult>();

            const string query = @"
                SELECT
                    types.Type,
                    types.TypeName,
                    COUNT(c.Id) AS NumberOfClients
                FROM
                (
                    SELECT 1 AS Type, 'Individual' AS TypeName
                    UNION ALL
                    SELECT 2 AS Type, 'Organization' AS TypeName
                ) AS types
                LEFT JOIN dbo.Clients c
                    ON c.Type = types.Type
                WHERE
                    @Type IS NULL
                    OR types.Type = @Type
                GROUP BY
                    types.Type,
                    types.TypeName
                ORDER BY
                    types.Type;";

            using var connection =
                new SqlConnection(_connectionString);

            using var command =
                new SqlCommand(query, connection);

            command.Parameters.Add(
                "@Type",
                SqlDbType.Int
            ).Value = type.HasValue
                ? type.Value
                : DBNull.Value;

            connection.Open();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                results.Add(new ClientTypeReportResult
                {
                    Type = Convert.ToInt32(reader["Type"]),

                    TypeName =
                        reader["TypeName"].ToString()
                        ?? string.Empty,

                    NumberOfClients =
                        Convert.ToInt32(
                            reader["NumberOfClients"])
                });
            }

            return results;
        }


        public List<PhoneNumberStatusReportResult>
    GetPhoneNumbersPerDevice(
        int? deviceId,
        int? status)
        {
            var results =
                new List<PhoneNumberStatusReportResult>();

            const string query = @"
        WITH PhoneNumberStatuses AS
        (
            SELECT
                p.Id,
                p.DeviceId,
                CASE
                    WHEN EXISTS
                    (
                        SELECT 1
                        FROM dbo.PhoneNumberReservations r
                        WHERE r.PhoneNumberId = p.Id
                          AND GETDATE() >= r.BED
                          AND
                          (
                              r.EED IS NULL
                              OR GETDATE() < r.EED
                          )
                    )
                    THEN 1
                    ELSE 2
                END AS Status
            FROM dbo.PhoneNumbers p
        ),
        Statuses AS
        (
            SELECT
                1 AS Status,
                'Reserved' AS StatusName

            UNION ALL

            SELECT
                2 AS Status,
                'Unreserved' AS StatusName
        )
        SELECT
            d.Id AS DeviceId,
            d.Name AS DeviceName,
            s.Status,
            s.StatusName,
            COUNT(pns.Id) AS NumberOfPhoneNumbers
        FROM dbo.Devices d
        CROSS JOIN Statuses s
        LEFT JOIN PhoneNumberStatuses pns
            ON pns.DeviceId = d.Id
            AND pns.Status = s.Status
        WHERE
            (@DeviceId IS NULL OR d.Id = @DeviceId)
            AND
            (@Status IS NULL OR s.Status = @Status)
        GROUP BY
            d.Id,
            d.Name,
            s.Status,
            s.StatusName
        ORDER BY
            d.Name,
            s.Status;";

            using var connection =
                new SqlConnection(_connectionString);

            using var command =
                new SqlCommand(query, connection);

            command.Parameters.Add(
                "@DeviceId",
                SqlDbType.Int
            ).Value = deviceId.HasValue
                ? deviceId.Value
                : DBNull.Value;

            command.Parameters.Add(
                "@Status",
                SqlDbType.Int
            ).Value = status.HasValue
                ? status.Value
                : DBNull.Value;

            connection.Open();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                results.Add(
                    new PhoneNumberStatusReportResult
                    {
                        DeviceId =
                            Convert.ToInt32(
                                reader["DeviceId"]),

                        DeviceName =
                            reader["DeviceName"].ToString()
                            ?? string.Empty,

                        Status =
                            Convert.ToInt32(
                                reader["Status"]),

                        StatusName =
                            reader["StatusName"].ToString()
                            ?? string.Empty,

                        NumberOfPhoneNumbers =
                            Convert.ToInt32(
                                reader["NumberOfPhoneNumbers"])
                    }
                );
            }

            return results;
        }
    }
}
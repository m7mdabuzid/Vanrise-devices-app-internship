using System;
using System.Collections.Generic;
using System.Data;
using Devices_tasks1_2_3.Models;
using Microsoft.Data.SqlClient;

namespace Devices_tasks1_2_3.Data
{
    public class PhoneNumberRepository
    {
        private readonly string _connectionString;

        public PhoneNumberRepository(
            IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString(
                    "DevicesConnection")
                ?? throw new InvalidOperationException(
                    "DevicesConnection was not found.");
        }

        public List<PhoneNumberResult>
            GetAllPhoneNumbers()
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
                ORDER BY p.Id;";

            return ExecutePhoneNumberQuery(query);
        }

        public List<PhoneNumberResult>
            GetFilteredPhoneNumbers(
                string? number,
                int? deviceId)
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
                WHERE
                    (
                        @Number = ''
                        OR p.Number LIKE
                            '%' + @Number + '%'
                    )
                    AND
                    (
                        @DeviceId IS NULL
                        OR p.DeviceId = @DeviceId
                    )
                ORDER BY p.Id;";

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(query, connection);

            command.Parameters.Add(
                "@Number",
                SqlDbType.NVarChar,
                30
            ).Value =
                (number ?? string.Empty).Trim();

            command.Parameters.Add(
                "@DeviceId",
                SqlDbType.Int
            ).Value =
                deviceId.HasValue
                    ? deviceId.Value
                    : DBNull.Value;

            connection.Open();

            using SqlDataReader reader =
                command.ExecuteReader();

            return PhoneNumberMapper
                .MapPhoneNumbers(reader);
        }

        public int AddPhoneNumber(
            Models.PhoneNumber phoneNumber)
        {
            const string query = @"
                IF EXISTS
                (
                    SELECT 1
                    FROM dbo.PhoneNumbers
                    WHERE Number = @Number
                )
                BEGIN
                    THROW 50001,
                        'This phone number already exists.',
                        1;
                END;

                INSERT INTO dbo.PhoneNumbers
                (
                    Number,
                    DeviceId
                )
                OUTPUT INSERTED.Id
                VALUES
                (
                    @Number,
                    @DeviceId
                );";

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(query, connection);

            AddParameters(command, phoneNumber);

            connection.Open();

            return Convert.ToInt32(
                command.ExecuteScalar()
            );
        }

        public bool UpdatePhoneNumber(
            Models.PhoneNumber phoneNumber)
        {
            const string query = @"
                IF EXISTS
                (
                    SELECT 1
                    FROM dbo.PhoneNumbers
                    WHERE Number = @Number
                      AND Id <> @Id
                )
                BEGIN
                    THROW 50001,
                        'This phone number already exists.',
                        1;
                END;

                UPDATE dbo.PhoneNumbers
                SET
                    Number = @Number,
                    DeviceId = @DeviceId
                WHERE Id = @Id;";

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(query, connection);

            command.Parameters.Add(
                "@Id",
                SqlDbType.Int
            ).Value = phoneNumber.Id;

            AddParameters(command, phoneNumber);

            connection.Open();

            return command.ExecuteNonQuery() > 0;
        }

        public bool DeletePhoneNumber(int id)
        {
            const string activeReservationQuery = @"
                SELECT COUNT(*)
                FROM dbo.PhoneNumberReservations
                WHERE PhoneNumberId = @Id
                  AND GETDATE() >= BED
                  AND
                  (
                      EED IS NULL
                      OR GETDATE() < EED
                  );";

            const string deleteHistoryQuery = @"
                DELETE FROM dbo.PhoneNumberReservations
                WHERE PhoneNumberId = @Id;";

            const string deletePhoneNumberQuery = @"
                DELETE FROM dbo.PhoneNumbers
                WHERE Id = @Id;";

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            connection.Open();

            using SqlTransaction transaction =
                connection.BeginTransaction();

            try
            {
                using SqlCommand activeCommand =
                    new SqlCommand(
                        activeReservationQuery,
                        connection,
                        transaction
                    );

                activeCommand.Parameters.Add(
                    "@Id",
                    SqlDbType.Int
                ).Value = id;

                int activeReservationCount =
                    Convert.ToInt32(
                        activeCommand.ExecuteScalar()
                    );

                if (activeReservationCount > 0)
                {
                    throw new InvalidOperationException(
                        "The phone number cannot be deleted while it has an active reservation."
                    );
                }

                using SqlCommand historyCommand =
                    new SqlCommand(
                        deleteHistoryQuery,
                        connection,
                        transaction
                    );

                historyCommand.Parameters.Add(
                    "@Id",
                    SqlDbType.Int
                ).Value = id;

                historyCommand.ExecuteNonQuery();

                using SqlCommand deleteCommand =
                    new SqlCommand(
                        deletePhoneNumberQuery,
                        connection,
                        transaction
                    );

                deleteCommand.Parameters.Add(
                    "@Id",
                    SqlDbType.Int
                ).Value = id;

                int deletedRows =
                    deleteCommand.ExecuteNonQuery();

                transaction.Commit();

                return deletedRows > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private List<PhoneNumberResult>
            ExecutePhoneNumberQuery(string query)
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(query, connection);

            connection.Open();

            using SqlDataReader reader =
                command.ExecuteReader();

            return PhoneNumberMapper
                .MapPhoneNumbers(reader);
        }

        private static void AddParameters(
            SqlCommand command,
            Models.PhoneNumber phoneNumber)
        {
            command.Parameters.Add(
                "@Number",
                SqlDbType.NVarChar,
                30
            ).Value =
                phoneNumber.Number.Trim();

            command.Parameters.Add(
                "@DeviceId",
                SqlDbType.Int
            ).Value =
                phoneNumber.DeviceId;
        }
    }
}
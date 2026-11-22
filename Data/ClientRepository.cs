using Devices_tasks1_2_3.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Devices_tasks1_2_3.Data
{
    public class ClientRepository
    {
        private readonly string _connectionString;

        public ClientRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DevicesConnection")
                ?? throw new InvalidOperationException(
                    "DevicesConnection was not found.");
        }

        public List<Client> GetAllClients()
        {
            const string query = @"
    SELECT
        c.Id,
        c.Name,
        c.Type,
        c.BirthDate,
        CAST(
            CASE WHEN EXISTS
            (
                SELECT 1
                FROM dbo.PhoneNumberReservations r
                WHERE r.ClientId = c.Id
                  AND GETDATE() >= r.BED
                  AND (r.EED IS NULL OR GETDATE() < r.EED)
            )
            THEN 1 ELSE 0 END
            AS BIT
        ) AS HasActiveReservation
    FROM dbo.Clients c
    ORDER BY c.Id;";

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(query, connection);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            return ClientMapper.MapClients(reader);
        }

        public List<Client> GetFilteredClients(
            string? name,
            int? type)
        {
            const string query = @"
    SELECT
        c.Id,
        c.Name,
        c.Type,
        c.BirthDate,
        CAST(
            CASE WHEN EXISTS
            (
                SELECT 1
                FROM dbo.PhoneNumberReservations r
                WHERE r.ClientId = c.Id
                  AND GETDATE() >= r.BED
                  AND (r.EED IS NULL OR GETDATE() < r.EED)
            )
            THEN 1 ELSE 0 END
            AS BIT
        ) AS HasActiveReservation
    FROM dbo.Clients c
    WHERE
        (@Name = '' OR c.Name LIKE '%' + @Name + '%')
        AND
        (@Type IS NULL OR c.Type = @Type)
    ORDER BY c.Id;";

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(query, connection);

            command.Parameters.Add(
                "@Name",
                SqlDbType.NVarChar,
                100
            ).Value = (name ?? string.Empty).Trim();

            command.Parameters.Add(
                "@Type",
                SqlDbType.Int
            ).Value = type.HasValue
                ? type.Value
                : DBNull.Value;

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            return ClientMapper.MapClients(reader);
        }

        public int AddClient(Client client)
        {
            const string query = @"
    IF @Type = 2
       AND EXISTS
       (
           SELECT 1
           FROM dbo.Clients
           WHERE Type = 2
             AND LTRIM(RTRIM(Name)) =
                 LTRIM(RTRIM(@Name))
       )
    BEGIN
        ;THROW 50002,
            'An organization with this name already exists.',
            1;
    END;

    INSERT INTO dbo.Clients
    (
        Name,
        Type,
        BirthDate
    )
    OUTPUT INSERTED.Id
    VALUES
    (
        @Name,
        @Type,
        @BirthDate
    );";

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(query, connection);

            AddClientParameters(command, client);

            connection.Open();

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public bool UpdateClient(Client client)
        {
            const string query = @"
    IF @Type = 2
       AND EXISTS
       (
           SELECT 1
           FROM dbo.Clients
           WHERE Type = 2
             AND LTRIM(RTRIM(Name)) =
                 LTRIM(RTRIM(@Name))
             AND Id <> @Id
       )
    BEGIN
        ;THROW 50002,
            'An organization with this name already exists.',
            1;
    END;

    UPDATE dbo.Clients
    SET
        Name = @Name,
        Type = @Type,
        BirthDate = @BirthDate
    WHERE Id = @Id;";

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(query, connection);

            command.Parameters.Add(
                "@Id",
                SqlDbType.Int
            ).Value = client.Id;

            AddClientParameters(command, client);

            connection.Open();

            return command.ExecuteNonQuery() > 0;
        }

        private static void AddClientParameters(
            SqlCommand command,
            Client client)
        {
            command.Parameters.Add(
                "@Name",
                SqlDbType.NVarChar,
                100
            ).Value = client.Name.Trim();

            command.Parameters.Add(
                "@Type",
                SqlDbType.Int
            ).Value = (int)client.Type;

            command.Parameters.Add(
                "@BirthDate",
                SqlDbType.Date
            ).Value = client.Type == ClientType.Organization ||
                     !client.BirthDate.HasValue
                ? DBNull.Value
                : client.BirthDate.Value.Date;
        }
        public bool DeleteClient(int id)
        {
            const string activeReservationQuery = @"
        SELECT COUNT(*)
        FROM dbo.PhoneNumberReservations
        WHERE ClientId = @Id
          AND GETDATE() >= BED
          AND
          (
              EED IS NULL
              OR GETDATE() < EED
          );";

            const string deleteReservationsQuery = @"
        DELETE FROM dbo.PhoneNumberReservations
        WHERE ClientId = @Id;";

            const string deleteClientQuery = @"
        DELETE FROM dbo.Clients
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
                        "The client cannot be deleted while an active phone-number reservation exists."
                    );
                }

                using SqlCommand deleteReservationsCommand =
                    new SqlCommand(
                        deleteReservationsQuery,
                        connection,
                        transaction
                    );

                deleteReservationsCommand.Parameters.Add(
                    "@Id",
                    SqlDbType.Int
                ).Value = id;

                deleteReservationsCommand.ExecuteNonQuery();

                using SqlCommand deleteClientCommand =
                    new SqlCommand(
                        deleteClientQuery,
                        connection,
                        transaction
                    );

                deleteClientCommand.Parameters.Add(
                    "@Id",
                    SqlDbType.Int
                ).Value = id;

                int deletedRows =
                    deleteClientCommand.ExecuteNonQuery();

                transaction.Commit();

                return deletedRows > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
using Devices_tasks1_2_3.Models;
using Devices_tasks1_2_3.Security;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Devices_tasks1_2_3.Repositories
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString(
                    "DevicesConnection")
                ?? throw new InvalidOperationException(
                    "DevicesConnection was not found.");
        }

        public User? ValidateLogin(
            string username,
            string password)
        {
            string passwordHash =
                PasswordHasher.HashPassword(password);

            const string query = @"
                SELECT
                    Id,
                    Username,
                    PasswordHash
                FROM dbo.Users
                WHERE Username = @Username
                  AND PasswordHash = @PasswordHash;";

            using var connection =
                new SqlConnection(_connectionString);

            using var command =
                new SqlCommand(query, connection);

            command.Parameters.Add(
                "@Username",
                SqlDbType.NVarChar,
                100
            ).Value = username;

            command.Parameters.Add(
                "@PasswordHash",
                SqlDbType.Char,
                64
            ).Value = passwordHash;

            connection.Open();

            using var reader = command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return new User
            {
                Id = Convert.ToInt32(reader["Id"]),

                Username =
                    reader["Username"].ToString()
                    ?? string.Empty,

                PasswordHash =
                    reader["PasswordHash"].ToString()
                    ?? string.Empty
            };
        }
    }
}
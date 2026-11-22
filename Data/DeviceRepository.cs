using Devices_tasks1_2_3.Models;
using Microsoft.Data.SqlClient;
using Devices_tasks1_2_3.Models;

namespace Devices_tasks1_2_3.Data
{
    public class DeviceRepository
    {
        private readonly string _connectionString;

        public DeviceRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DevicesConnection")
                ?? throw new InvalidOperationException(
                    "DevicesConnection was not found in appsettings.json.");
        }

        // GET ALL DEVICES
        public List<Device> GetAllDevices()
        {
            const string query = @"
                SELECT Id, Name
                FROM dbo.Devices
                ORDER BY Id;";

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(query, connection);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            return DeviceMapper.MapDevices(
                reader,
                DeviceMapper.AsFunc(DeviceMapper.MapDevice),
                "Id",
                "Name"
            );
        }

        // GET FILTERED DEVICES
        public List<Device> GetFilteredDevices(string? searchValue)
        {
            const string query = @"
                SELECT Id, Name
                FROM dbo.Devices
                WHERE @SearchValue = ''
                   OR Name LIKE '%' + @SearchValue + '%'
                   OR Id = TRY_CONVERT(INT, @SearchValue)
                ORDER BY Id;";

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@SearchValue",
                (searchValue ?? string.Empty).Trim()
            );

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            return DeviceMapper.MapDevices(
                reader,
                DeviceMapper.AsFunc(DeviceMapper.MapDevice),
                "Id",
                "Name"
            );
        }

        // ADD DEVICE
        public int AddDevice(Device device)
        {
            const string query = @"
                INSERT INTO dbo.Devices (Name)
                OUTPUT INSERTED.Id
                VALUES (@Name);";

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@Name",
                device.Name.Trim()
            );

            connection.Open();

            object? result = command.ExecuteScalar();

            return Convert.ToInt32(result);
        }

        // UPDATE DEVICE
        public bool UpdateDevice(Device device)
        {
            const string query = @"
                UPDATE dbo.Devices
                SET Name = @Name
                WHERE Id = @Id;";

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", device.Id);
            command.Parameters.AddWithValue(
                "@Name",
                device.Name.Trim()
            );

            connection.Open();

            int affectedRows = command.ExecuteNonQuery();

            return affectedRows > 0;
        }
        //DELETE DEVICES
        public bool DeleteDevice(int id)
        {
            const string query = @"
        DELETE FROM dbo.Devices
        WHERE Id = @Id;";

            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = new(query, connection);

            command.Parameters.AddWithValue("@Id", id);

            connection.Open();

            return command.ExecuteNonQuery() > 0;
        }

    }
}
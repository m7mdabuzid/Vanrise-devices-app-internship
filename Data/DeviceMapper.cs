using Devices_tasks1_2_3.Models;
using Microsoft.Data.SqlClient;
using Devices_tasks1_2_3.Models;

namespace Devices_tasks1_2_3.Data
{
    public static class DeviceMapper
    {
        // Custom delegate: represents a method that converts
        // one SQL row into one Device object.
        public delegate Device DeviceMapDelegate(SqlDataReader reader);

        // Maps the current row of SqlDataReader into a Device.
        public static Device MapDevice(SqlDataReader reader)
        {
            return new Device
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name"))
            };
        }

        // Converts our custom delegate into a Func.
        public static Func<SqlDataReader, Device> AsFunc(
            DeviceMapDelegate mapper)
        {
            return reader => mapper(reader);
        }

        // Maps all rows returned from SQL into a list of devices.
        // params allows us to pass several required column names.
        public static List<Device> MapDevices(
            SqlDataReader reader,
            Func<SqlDataReader, Device> mapFunction,
            params string[] requiredColumns)
        {
            ValidateColumns(reader, requiredColumns);

            var devices = new List<Device>();

            while (reader.Read())
            {
                Device device = mapFunction(reader);
                devices.Add(device);
            }

            return devices;
        }

        private static void ValidateColumns(
            SqlDataReader reader,
            params string[] requiredColumns)
        {
            foreach (string columnName in requiredColumns)
            {
                // Throws an error if the SQL result does not contain
                // the required column.
                reader.GetOrdinal(columnName);
            }
        }
    }
}
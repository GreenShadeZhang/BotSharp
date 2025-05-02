using BotSharp.Plugin.ESP32.Repository.Entities;
using BotSharp.Plugin.ESP32.Settings;
using LiteDB;
using System.Globalization;

namespace BotSharp.Plugin.ESP32.Repository.LiteDB
{

    public class LiteDBIoTDbContext : IDisposable
    {
        private readonly LiteDatabase _liteDBClient;
        private readonly string _collectionPrefix;
        public LiteDBIoTDbContext(ESP32Setting dbSettings)
        {
            var mapper = new BsonMapper();
            mapper.RegisterType<DateTime>(
                value => value.ToString("o", CultureInfo.InvariantCulture),
                bson => DateTime.ParseExact
                (bson, "o", CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind));

            mapper.RegisterType<DateTimeOffset>(
                value => value.ToString("o", CultureInfo.InvariantCulture),
                bson => DateTimeOffset.ParseExact
                (bson, "o", CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind));

            var dbConnectionString = dbSettings.LiteDB;
            var connectionString = new ConnectionString(dbConnectionString)
            {
                Connection = ConnectionType.Shared
            };
            _liteDBClient = new LiteDatabase(connectionString, mapper);

            _collectionPrefix = dbSettings.TablePrefix.IfNullOrEmptyAs("IoT");
        }

        private LiteDatabase Database { get { return _liteDBClient; } }


        public void Dispose()
        {
            _liteDBClient?.Dispose();
        }
        public ILiteCollection<IoTDevice> IoTDevices
            => Database.GetCollection<IoTDevice>($"{_collectionPrefix}_IoTDevices");

    }
}

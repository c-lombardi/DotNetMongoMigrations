using Migrations.Types;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoMigrations.Implemented;
namespace MongoMigrations
{
    public static class MigrationRunner
    {
        private static readonly string VersionCollectionName = "DatabaseVersion";
        private static readonly string SessionCollectionName = "DatabaseMigrationSession";

        static MigrationRunner()
        {
            BsonSerializer.RegisterSerializer(typeof(MigrationVersion), new MigrationVersionSerializer());
        }

        public static void UpdateToLatest(string mongoServerLocation, string databaseName)
        {
            Migrations.MigrationRunner.UpdateToLatest(
                new MigrationLocator(),
                new MongoDatabaseMigrationStatus(
                    new MongoClient(mongoServerLocation).GetDatabase(databaseName),
                    VersionCollectionName,
                    SessionCollectionName));
        }
    }
}
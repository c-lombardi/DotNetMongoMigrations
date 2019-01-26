using Migrations.ToBeImplemented;
using Migrations.Types;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoMigrations.Implemented;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace MongoMigrations
{
    public static class MigrationRunnerAsync
    {
        private static readonly string VersionCollectionName = "DatabaseVersion";
        private static readonly string SessionCollectionName = "DatabaseMigrationSession";

        static MigrationRunnerAsync()
        {
            BsonSerializer.RegisterSerializer(typeof(MigrationVersion), new MigrationVersionSerializer());
        }

        public static Task UpdateToLatestAsync(
            string mongoServerLocation,
            string databaseName,
            IEnumerable<Assembly> assemblies,
            IRepositoryToMigrate repositoryToMigrate)
        {
            return Migrations.MigrationRunnerAsync.UpdateToLatestAsync(
                new MigrationLocatorAsync(assemblies),
                new MongoDatabaseMigrationStatusAsync(
                    new MongoClient(mongoServerLocation).GetDatabase(databaseName),
                    VersionCollectionName,
                    SessionCollectionName),
                repositoryToMigrate);
        }

        public static Task UpdateToLatestAsync(
            string mongoServerLocation,
            string databaseName,
            IEnumerable<Assembly> assemblies)
        {
            return Migrations.MigrationRunnerAsync.UpdateToLatestAsync(
                new MigrationLocatorAsync(assemblies),
                new MongoDatabaseMigrationStatusAsync(
                    new MongoClient(mongoServerLocation).GetDatabase(databaseName),
                    VersionCollectionName,
                    SessionCollectionName));
        }

        public static Task UpdateToLatestAsync(
            string mongoConnectionString,
            IEnumerable<Assembly> assemblies,
            IRepositoryToMigrate repositoryToMigrate)
        {
            return Migrations.MigrationRunnerAsync.UpdateToLatestAsync(
                new MigrationLocatorAsync(assemblies),
                new MongoDatabaseMigrationStatusAsync(
                    new MongoClient(mongoConnectionString)
                        .GetDatabase(MongoUrl.Create(mongoConnectionString).DatabaseName),
                    VersionCollectionName,
                    SessionCollectionName),
                repositoryToMigrate);
        }

        public static Task UpdateToLatestAsync(
            string mongoConnectionString,
            IEnumerable<Assembly> assemblies)
        {
            return Migrations.MigrationRunnerAsync.UpdateToLatestAsync(
                new MigrationLocatorAsync(assemblies),
                new MongoDatabaseMigrationStatusAsync(
                    new MongoClient(mongoConnectionString)
                        .GetDatabase(MongoUrl.Create(mongoConnectionString).DatabaseName),
                    VersionCollectionName,
                    SessionCollectionName));
        }
    }
}
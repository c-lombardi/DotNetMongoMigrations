using Migrations.ToBeImplemented;
using Migrations.Types;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoMigrations.Implemented;
using System.Collections.Generic;
using System.Reflection;

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

        public static void UpdateToLatest(
            string mongoServerLocation,
            string databaseName,
            IEnumerable<Assembly> assemblies,
            IRepositoryToMigrate repositoryToMigrate)
        {
            Migrations.MigrationRunner.UpdateToLatest(
                new MigrationLocator(assemblies),
                new MongoDatabaseMigrationStatus(
                    new MongoClient(mongoServerLocation).GetDatabase(databaseName),
                    VersionCollectionName,
                    SessionCollectionName),
                repositoryToMigrate);
        }

        public static void UpdateToLatest(
            string mongoServerLocation,
            string databaseName,
            IEnumerable<Assembly> assemblies)
        {
            Migrations.MigrationRunner.UpdateToLatest(
                new MigrationLocator(assemblies),
                new MongoDatabaseMigrationStatus(
                    new MongoClient(mongoServerLocation).GetDatabase(databaseName),
                    VersionCollectionName,
                    SessionCollectionName));
        }

        public static void UpdateToLatest(
            string mongoConnectionString,
            IEnumerable<Assembly> assemblies,
            IRepositoryToMigrate repositoryToMigrate)
        {
            Migrations.MigrationRunner.UpdateToLatest(
                new MigrationLocator(assemblies),
                new MongoDatabaseMigrationStatus(
                    new MongoClient(mongoConnectionString)
                        .GetDatabase(MongoUrl.Create(mongoConnectionString).DatabaseName),
                    VersionCollectionName,
                    SessionCollectionName),
                repositoryToMigrate);
        }
    }
}
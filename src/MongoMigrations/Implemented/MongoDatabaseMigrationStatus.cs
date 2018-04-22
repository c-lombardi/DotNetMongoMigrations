using System.Linq;
using MongoDB.Driver;
using Migrations.ToBeImplemented;
using Migrations.Types;
using System.Collections.Generic;

namespace MongoMigrations.Implemented
{
    public class MongoDatabaseMigrationStatus : IRepository
    {
        public readonly IMongoDatabase MongoDatabase;

        public readonly string VersionCollectionName;
        public readonly string SessionCollectionName;

        public MongoDatabaseMigrationStatus(
            IMongoDatabase mongoDatabase,
            string versionCollectionName,
            string sessionCollectionName)
        {
            MongoDatabase = mongoDatabase;
            VersionCollectionName = versionCollectionName;
            SessionCollectionName = sessionCollectionName;
        }

        public virtual IEnumerable<MigrationSession> GetMigrationSessions()
        {
            return MongoDatabase.GetCollection<MigrationSession>(SessionCollectionName)
                .Find(Builders<MigrationSession>.Filter.Empty)
                .ToList();
        }

        public virtual IEnumerable<RepositoryMigration> GetMigrations()
        {
            return MongoDatabase.GetCollection<RepositoryMigration>(VersionCollectionName)
                .Find(Builders<RepositoryMigration>.Filter.Empty)
                .ToList();
        }

        public virtual void AddMigrationSession(MigrationSession migrationSession)
        {
            MongoDatabase.GetCollection<MigrationSession>(SessionCollectionName).InsertOne(migrationSession);
        }

        public virtual void AddMigration(RepositoryMigration migration)
        {
            MongoDatabase.GetCollection<RepositoryMigration>(VersionCollectionName).InsertOne(migration);
        }

        public virtual void UpsertMigrationSession(MigrationSession migrationSession)
        {
            MongoDatabase.GetCollection<MigrationSession>(VersionCollectionName).ReplaceOne(x => x.MigrationSessionId == migrationSession.MigrationSessionId, migrationSession);
        }

        public virtual void UpsertMigration(RepositoryMigration migration)
        {
            MongoDatabase.GetCollection<RepositoryMigration>(VersionCollectionName).ReplaceOne(x => x.Version == migration.Version, migration);
        }

        public virtual void DeleteMigration(RepositoryMigration migration)
        {
            MongoDatabase.GetCollection<RepositoryMigration>(VersionCollectionName).DeleteOne(x => x.Version == migration.Version);
        }
    }
}
using System.Linq;
using MongoDB.Driver;
using Migrations.ToBeImplemented;
using Migrations.Types;
using System.Collections.Generic;
using MongoMigrations.Implemented.Types;

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
            return MongoDatabase.GetCollection<MongoRepositoryMigration>(VersionCollectionName)
                .Find(Builders<MongoRepositoryMigration>.Filter.Empty)
                .ToList()
                .Select(mongoRepositoryMigration =>
                    new RepositoryMigration(null)
                    {
                        Description = mongoRepositoryMigration.Description,
                        StartedOn = mongoRepositoryMigration.StartedOn,
                        CompletedOn = mongoRepositoryMigration.CompletedOn,
                        Version = mongoRepositoryMigration.Version
                    });
        }

        public virtual void AddMigrationSession(MigrationSession migrationSession)
        {
            MongoDatabase.GetCollection<MigrationSession>(SessionCollectionName).InsertOne(migrationSession);
        }

        public virtual void AddMigration(RepositoryMigration repositoryMigration)
        {
            MongoDatabase.GetCollection<MongoRepositoryMigration>(VersionCollectionName).InsertOne(new MongoRepositoryMigration(repositoryMigration));
        }

        public virtual void UpsertMigrationSession(MigrationSession migrationSession)
        {
            MongoDatabase.GetCollection<MigrationSession>(VersionCollectionName).ReplaceOne(x => x.MigrationSessionId == migrationSession.MigrationSessionId, migrationSession);
        }

        public virtual void UpsertMigration(RepositoryMigration repositoryMigration)
        {
            MongoDatabase.GetCollection<MongoRepositoryMigration>(VersionCollectionName).ReplaceOne(x => x.Version == repositoryMigration.Version, new MongoRepositoryMigration(repositoryMigration));
        }

        public virtual void DeleteMigration(RepositoryMigration repositoryMigration)
        {
            MongoDatabase.GetCollection<MongoRepositoryMigration>(VersionCollectionName).DeleteOne(x => x.Version == repositoryMigration.Version);
        }
    }
}
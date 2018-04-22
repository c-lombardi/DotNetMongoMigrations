using System.Linq;
using MongoDB.Driver;
using Migrations.ToBeImplemented;
using Migrations.Types;
using System.Collections.Generic;
using MongoMigrations.Implemented.Types;
using MongoDB.Bson;

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

            if (!CollectionExists(mongoDatabase, sessionCollectionName))
            {
                mongoDatabase.CreateCollection(sessionCollectionName);
            }

            if (!CollectionExists(mongoDatabase, versionCollectionName))
            {
                mongoDatabase.CreateCollection(versionCollectionName);
            }
        }

        private static bool CollectionExists(IMongoDatabase mongoDatabase, string collectionName)
        {
            BsonDocument filter = new BsonDocument("name", collectionName);
            //filter by collection name
            IAsyncCursor<BsonDocument> collections = mongoDatabase.ListCollections(
                new ListCollectionsOptions
                {
                    Filter = filter
                });
            //check for existence
            return collections.Any();
        }

        public virtual IEnumerable<MigrationSession> GetMigrationSessions()
        {
            return MongoDatabase.GetCollection<MongoMigrationSession>(SessionCollectionName)
                .Find(Builders<MongoMigrationSession>.Filter.Empty)
                .ToList()
                .Select(mongoMigrationSession =>
                    new MigrationSession()
                    {
                        StartedOn = mongoMigrationSession.StartedOn,
                        CompletedOn = mongoMigrationSession.CompletedOn,
                        CompletedOnVersion = mongoMigrationSession.CompletedOnVersion,
                        CompletedSuccessfully = mongoMigrationSession.CompletedSuccessfully,
                        FirstVersion = mongoMigrationSession.FirstVersion,
                        InnerException = mongoMigrationSession.InnerException,
                        LastVersion = mongoMigrationSession.LastVersion,
                        MigrationSessionId = mongoMigrationSession.MigrationSessionId,
                        StackTrace = mongoMigrationSession.StackTrace
                    });
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
            MongoDatabase.GetCollection<MongoMigrationSession>(SessionCollectionName).InsertOne(new MongoMigrationSession(migrationSession));
        }

        public virtual void AddMigration(RepositoryMigration repositoryMigration)
        {
            MongoDatabase.GetCollection<MongoRepositoryMigration>(VersionCollectionName).InsertOne(new MongoRepositoryMigration(repositoryMigration));
        }

        public virtual void UpsertMigrationSession(MigrationSession migrationSession)
        {
            MongoDatabase.GetCollection<MongoMigrationSession>(VersionCollectionName).ReplaceOne(x => x.MigrationSessionId == migrationSession.MigrationSessionId, new MongoMigrationSession(migrationSession));
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
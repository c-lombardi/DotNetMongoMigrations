using Migrations.ToBeImplemented;
using Migrations.Types;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoMigrations.Implemented.Types;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoMigrations.Implemented
{
    public class MongoDatabaseMigrationStatusAsync : IRepositoryAsync
    {
        public readonly IMongoDatabase MongoDatabase;

        public readonly string VersionCollectionName;
        public readonly string SessionCollectionName;

        public MongoDatabaseMigrationStatusAsync(
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

        public virtual Task<IEnumerable<MigrationSessionAsync>> GetMigrationSessionsAsync()
        {
            return Task.FromResult(MongoDatabase.GetCollection<MongoMigrationSessionAsync>(SessionCollectionName)
                .Find(Builders<MongoMigrationSessionAsync>.Filter.Empty)
                .ToList()
                .Select(mongoMigrationSession =>
                    new MigrationSessionAsync()
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
                    }));
        }

        public virtual Task<IEnumerable<RepositoryMigrationAsync>> GetMigrationsAsync()
        {
            return Task.FromResult(MongoDatabase.GetCollection<MongoRepositoryMigrationAsync>(VersionCollectionName)
                .Find(Builders<MongoRepositoryMigrationAsync>.Filter.Empty)
                .ToList()
                .Select(mongoRepositoryMigration =>
                    new RepositoryMigrationAsync(null)
                    {
                        Description = mongoRepositoryMigration.Description,
                        StartedOn = mongoRepositoryMigration.StartedOn,
                        CompletedOn = mongoRepositoryMigration.CompletedOn,
                        Version = mongoRepositoryMigration.Version
                    }));
        }

        public virtual Task AddMigrationSessionAsync(MigrationSessionAsync migrationSession)
        {
            return MongoDatabase.GetCollection<MongoMigrationSessionAsync>(SessionCollectionName).InsertOneAsync(new MongoMigrationSessionAsync(migrationSession));
        }

        public virtual Task AddMigrationAsync(RepositoryMigrationAsync repositoryMigration)
        {
            return MongoDatabase.GetCollection<MongoRepositoryMigrationAsync>(VersionCollectionName).InsertOneAsync(new MongoRepositoryMigrationAsync(repositoryMigration));
        }

        public virtual Task UpsertMigrationSessionAsync(MigrationSessionAsync migrationSession)
        {
            return MongoDatabase.GetCollection<MongoMigrationSessionAsync>(SessionCollectionName).ReplaceOneAsync(x => x.MigrationSessionId == migrationSession.MigrationSessionId, new MongoMigrationSessionAsync(migrationSession));
        }

        public virtual Task UpsertMigrationAsync(RepositoryMigrationAsync repositoryMigration)
        {
            return MongoDatabase.GetCollection<MongoRepositoryMigrationAsync>(VersionCollectionName).ReplaceOneAsync(x => x.Version == repositoryMigration.Version, new MongoRepositoryMigrationAsync(repositoryMigration));
        }

        public virtual Task DeleteMigrationAsync(RepositoryMigrationAsync repositoryMigration)
        {
            return MongoDatabase.GetCollection<MongoRepositoryMigrationAsync>(VersionCollectionName).DeleteOneAsync(x => x.Version == repositoryMigration.Version);
        }
    }
}
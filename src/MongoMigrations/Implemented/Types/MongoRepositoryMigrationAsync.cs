using Migrations.Types;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MongoMigrations.Implemented.Types
{
    public class MongoRepositoryMigrationAsync
    {
        [BsonId]
        public MigrationVersion Version { get; set; }
        public string Description { get; set; }
        public DateTime StartedOn { get; set; }
        public DateTime? CompletedOn { get; set; }

        public MongoRepositoryMigrationAsync() { } 

        public MongoRepositoryMigrationAsync(RepositoryMigrationAsync repositoryMigration) : this()
        {
            Description = repositoryMigration.Description;
            StartedOn = repositoryMigration.StartedOn;
            Version = repositoryMigration.Version;
            CompletedOn = repositoryMigration.CompletedOn;
        }
    }
}

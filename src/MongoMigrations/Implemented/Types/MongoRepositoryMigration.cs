using Migrations.Types;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MongoMigrations.Implemented.Types
{
    public class MongoRepositoryMigration
    {
        [BsonId]
        public MigrationVersion Version { get; set; }
        public string Description { get; set; }
        public DateTime StartedOn { get; set; }
        public DateTime? CompletedOn { get; set; }

        public MongoRepositoryMigration() { } 

        public MongoRepositoryMigration(RepositoryMigration repositoryMigration) : this()
        {
            Description = repositoryMigration.Description;
            StartedOn = repositoryMigration.StartedOn;
            Version = repositoryMigration.Version;
            CompletedOn = repositoryMigration.CompletedOn;
        }
    }
}

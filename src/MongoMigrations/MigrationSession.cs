using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MongoMigrations
{
    public class MigrationSession
    {
        public MigrationSession()
        {
            MigrationSessionId = Guid.NewGuid();
            StartedOn = DateTime.UtcNow;
        }

        public MigrationSession(IEnumerable<Migration> migrations) : this()
        {
            if (migrations.Any())
            {
                FirstVersion = migrations.Min(migration => migration.Version);
                LastVersion = migrations.Max(migration => migration.Version);
            }
            else
            {
                FirstVersion = MigrationVersion.Default();
                LastVersion = MigrationVersion.Default();
            }
        }


        [BsonId]
        public Guid MigrationSessionId { get; set; }

        public MigrationVersion FirstVersion { get; set; }

        //This should only be set here.
        public MigrationVersion LastVersion { get; set; }

        //This should only be set here.
        public DateTime StartedOn { get; set; }
        public MigrationVersion CompletedOnVersion { get; set; }
        public DateTime? CompletedOn { get; set; }
        public bool CompletedSuccessfully { get; set; }
        public string InnerException { get; set; }
        public string StackTrace { get; set; }

        public override string ToString()
        {
            return $"Migration session started on {FirstVersion.ToString()} version at {StartedOn} and completed at {CompletedOn ?? DateTime.MinValue} with version {CompletedOnVersion.ToString()} with target version {LastVersion.ToString()} and was {_getSuccessfulString(CompletedSuccessfully)}.";
        }

        private static string _getSuccessfulString(bool completedSuccessfully)
        {
            return completedSuccessfully
                ? "successful"
                : "unsuccessful";
        }
    }
}

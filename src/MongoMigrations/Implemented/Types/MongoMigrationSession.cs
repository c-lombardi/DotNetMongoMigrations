using Migrations.Types;
using System;

namespace MongoMigrations.Implemented.Types
{
    public class MongoMigrationSession
    {
        public Guid MigrationSessionId { get; set; }
        public MigrationVersion FirstVersion { get; set; }
        public MigrationVersion LastVersion { get; set; }
        public DateTime StartedOn { get; set; }
        public MigrationVersion CompletedOnVersion { get; set; }
        public DateTime? CompletedOn { get; set; }
        public bool CompletedSuccessfully { get; set; }
        public string InnerException { get; set; }
        public string StackTrace { get; set; }

        public MongoMigrationSession(MigrationSession migrationSession)
        {
            MigrationSessionId = migrationSession.MigrationSessionId;
            FirstVersion = migrationSession.FirstVersion;
            LastVersion = migrationSession.LastVersion;
            StartedOn = migrationSession.StartedOn;
            CompletedOnVersion = migrationSession.CompletedOnVersion;
            CompletedOn = migrationSession.CompletedOn;
            CompletedSuccessfully = migrationSession.CompletedSuccessfully;
            InnerException = migrationSession.InnerException;
            StackTrace = migrationSession.StackTrace;
        }
    }
}

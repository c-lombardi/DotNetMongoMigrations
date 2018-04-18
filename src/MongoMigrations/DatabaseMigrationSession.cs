using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MongoMigrations
{
    public class DatabaseMigrationSession
    {
        private readonly MigrationRunner _Runner;

        public string VersionCollectionName = "DatabaseMigrationSession";

        public DatabaseMigrationSession(MigrationRunner runner)
        {
            _Runner = runner;
        }

        public virtual IMongoCollection<MigrationSession> GetMigrationSessions()
        {
            return _Runner.Database.GetCollection<MigrationSession>(VersionCollectionName);
        }

        public virtual bool MigrationSessionIsExecuting()
        {
            return GetMigrationSessions()
                .Find(Builders<MigrationSession>.Filter.Empty)
                .ToList()
                .Any(session => session.CompletedOn == null
                    && session.CompletedOnVersion == null);
        }

        public virtual void CompleteMigrationSession(MigrationSession migrationSession)
        {
            migrationSession.CompletedOn = DateTime.UtcNow;
            migrationSession.CompletedSuccessfully = true;
            migrationSession.CompletedOnVersion = migrationSession.LastVersion;
            GetMigrationSessions().ReplaceOne(x => x.MigrationSessionId == migrationSession.MigrationSessionId, migrationSession);
        }

        public virtual void FailMigrationSession(MigrationSession migrationSession, MigrationException migrationException)
        {
            migrationSession.CompletedOn = DateTime.UtcNow;
            migrationSession.CompletedSuccessfully = false;
            migrationSession.CompletedOnVersion = migrationException.VersionFailedOn;
            migrationSession.InnerException = migrationException?.InnerException?.Message ?? string.Empty;
            migrationSession.StackTrace = migrationException?.InnerException?.StackTrace ?? string.Empty;
            GetMigrationSessions().ReplaceOne(x => x.MigrationSessionId == migrationSession.MigrationSessionId, migrationSession);
        }

        public virtual MigrationSession StartMigrationSession(IEnumerable<Migration> migrations)
        {
            MigrationSession migrationSession = new MigrationSession(migrations);
            GetMigrationSessions().InsertOne(migrationSession);
            return migrationSession;
        }
    }
}

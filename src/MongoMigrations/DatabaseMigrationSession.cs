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
                .Any(session => session.CompletedOn == null);
        }

        public virtual void CompleteMigrationSession(MigrationSession migrationSession)
        {
            migrationSession.CompletedOn = DateTime.UtcNow;
            migrationSession.CompletedOnVersion = migrationSession.LastVersion;
            migrationSession.CompletedSuccessfully = true;
            GetMigrationSessions().ReplaceOne(x => x.MigrationSessionId == migrationSession.MigrationSessionId, migrationSession);
        }

        public virtual void FailMigrationSession(MigrationSession migrationSession, MigrationVersion completedOnVersion)
        {
            migrationSession.CompletedOn = DateTime.UtcNow;
            migrationSession.CompletedOnVersion = completedOnVersion;
            migrationSession.CompletedSuccessfully = false;
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

using MongoDB.Driver;
using System;
using System.Collections.Generic;

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

        public virtual void CompleteMigrationSession(MigrationSession migrationSession, MigrationVersion? completedOnVersion, bool successful)
        {
            migrationSession.CompletedOn = DateTime.Now;
            migrationSession.CompletedOnVersion = completedOnVersion ?? migrationSession.LastVersion;
            migrationSession.CompletedSuccessfully = successful;
            GetMigrationSessions().ReplaceOne(x => x.StartedOn == migrationSession.StartedOn, migrationSession);
        }

        public virtual MigrationSession StartMigrationSession(IEnumerable<Migration> migration)
        {
            MigrationSession migrationSession = new MigrationSession(migration);
            GetMigrationSessions().InsertOne(migrationSession);
            return migrationSession;
        }
    }
}

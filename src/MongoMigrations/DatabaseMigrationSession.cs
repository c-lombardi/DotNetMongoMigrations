﻿using MongoDB.Driver;
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

        public virtual void CompleteMigrationSession(MigrationSession migrationSession, MigrationVersion completedOnVersion, bool successful)
        {
            migrationSession.CompletedOn = DateTime.Now;
            migrationSession.CompletedOnVersion = successful ?
                migrationSession.LastVersion
                : completedOnVersion;
            migrationSession.CompletedSuccessfully = successful;
            GetMigrationSessions().ReplaceOne(x => x.StartedOn == migrationSession.StartedOn, migrationSession);
        }

        public virtual MigrationSession StartMigrationSession(IEnumerable<Migration> migrations)
        {
            MigrationSession migrationSession = new MigrationSession(migrations);
            GetMigrationSessions().InsertOne(migrationSession);
            return migrationSession;
        }
    }
}

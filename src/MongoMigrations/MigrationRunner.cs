namespace MongoMigrations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;

    public class MigrationRunner
    {
        static MigrationRunner()
        {
            Init();
        }

        public static void Init()
        {
            BsonSerializer.RegisterSerializer(typeof(MigrationVersion), new MigrationVersionSerializer());
        }

        public MigrationRunner(string mongoServerLocation, string databaseName)
            : this(new MongoClient(mongoServerLocation).GetDatabase(databaseName))
        {
        }

        public MigrationRunner(IMongoDatabase database)
        {
            Database = database;
            DatabaseStatus = new DatabaseMigrationStatus(this);
            DatabaseSession = new DatabaseMigrationSession(this);
            MigrationLocator = new MigrationLocator();
        }

        public IMongoDatabase Database { get; set; }
        public MigrationLocator MigrationLocator { get; set; }
        public DatabaseMigrationStatus DatabaseStatus { get; set; }
        public DatabaseMigrationSession DatabaseSession { get; private set; }

        public virtual void UpdateToLatest()
        {
            Console.WriteLine(WhatWeAreUpdating() + " to latest...");
            UpdateTo(MigrationLocator.LatestVersion());
        }

        private string WhatWeAreUpdating()
        {
            return string.Format("Updating server(s) \"{0}\" for database \"{1}\"", ServerAddresses(), DatabaseNamespace.Admin.DatabaseName);
        }

        private string ServerAddresses()
        {
            return String.Join(",", Database.Client.Settings.Servers.Select(server => String.Format("{0}{1}{2}:{3}{4}{5}", "{", server.Host, "}", "{", server.Port, "}")));
        }

        protected virtual void ApplyMigrations(IEnumerable<Migration> migrations)
        {
            if (DatabaseSession.TryStartMigrationSession(migrations, out MigrationSession migrationSession))
            {
                try
                {
                    foreach (Migration migration in migrations)
                    {
                        ApplyMigration(migration);
                    }
                    DatabaseSession.CompleteMigrationSession(migrationSession);
                }
                catch (MigrationException migrationException)
                {
                    DatabaseSession.FailMigrationSession(migrationSession, migrationException);
                    throw migrationException;
                }
            }
        }

        protected virtual void ApplyMigration(Migration migration)
        {
            Console.WriteLine(new { Message = "Applying migration", migration.Version, migration.Description, DatabaseName = Database.DatabaseNamespace.DatabaseName });

            var appliedMigration = DatabaseStatus.StartMigration(migration);
            migration.Database = Database;
            try
            {
                migration.Update();
                DatabaseStatus.CompleteMigration(appliedMigration);
            }
            catch (Exception exception)
            {
                OnMigrationException(migration, exception);
            }
        }

        protected virtual void OnRollbackException(Migration migration, Exception exception)
        {
            throw new MigrationException(exception.Message, exception, migration.Version);
        }

        protected virtual void OnMigrationException(Migration migration, Exception exception)
        {
            try
            {
                migration.Rollback();
                DatabaseStatus.DeleteMigration(new AppliedMigration(migration));
                throw new MigrationException(exception.Message, exception, migration.Version);
            }
            catch (Exception rollbackEx)
            {
                OnRollbackException(migration, rollbackEx);
            }
        }

        public virtual void UpdateTo(MigrationVersion updateToVersion)
        {
            var currentVersion = DatabaseStatus.GetLastAppliedMigration();
            Console.WriteLine(new { Message = WhatWeAreUpdating(), currentVersion, updateToVersion, DatabaseName = Database.DatabaseNamespace.DatabaseName });

            var migrations = MigrationLocator.GetMigrationsAfter(currentVersion)
                                             .Where(m => m.Version <= updateToVersion);

            ApplyMigrations(migrations);
        }
    }
}
namespace MongoMigrations
{
    using System;

    public class MigrationException : ApplicationException
    {
        public MigrationVersion? VersionFailedOn { get; private set; }
        public MigrationException(string message, Exception innerException, MigrationVersion? versionFailedOn) : base(message, innerException)
        {
            VersionFailedOn = versionFailedOn;
        }
    }
}
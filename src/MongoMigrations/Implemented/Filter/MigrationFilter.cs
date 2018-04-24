using Migrations.Types.ToBeImplemented;

namespace MongoMigrations.Implemented.Filter
{
    public abstract class MigrationFilter
    {
        public abstract bool Exclude(Migration migration);
    }
}
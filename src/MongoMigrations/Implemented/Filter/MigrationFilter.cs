using Migrations.Types;

namespace MongoMigrations.Implemented.Filter
{
    public abstract class MigrationFilter
    {
        public abstract bool Exclude(Migration migration);
    }
}
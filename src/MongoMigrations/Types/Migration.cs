using Migrations.Types;
using MongoDB.Driver;

namespace MongoMigrations.Types
{
    public abstract class Migration : Migrations.Types.Migration
    {
        protected readonly IMongoDatabase MongoDatabase;
        public Migration(
            IMongoDatabase mongoDatabase, 
            MigrationVersion version, 
            string description) : base(version, description)
        {
        }
    }
}

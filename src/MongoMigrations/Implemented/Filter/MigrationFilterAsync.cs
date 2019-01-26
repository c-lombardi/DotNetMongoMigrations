using Migrations.Types.ToBeImplemented;
using System.Threading.Tasks;

namespace MongoMigrations.Implemented.Filter
{
    public abstract class MigrationFilterAsync
    {
        public abstract Task<bool> ExcludeAsync(MigrationAsync migration);
    }
}
using Migrations.Types.ToBeImplemented;
using System.Linq;
using System.Threading.Tasks;

namespace MongoMigrations.Implemented.Filter.Experimental
{
    public class ExcludeExperimentalMigrationsAsync : MigrationFilterAsync
    {
        public override Task<bool> ExcludeAsync(MigrationAsync migration)
        {
            if (migration == null)
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(migration.GetType()
                .GetCustomAttributes(true)
                .OfType<ExperimentalAttributeAsync>()
                .Any());
        }
    }
}
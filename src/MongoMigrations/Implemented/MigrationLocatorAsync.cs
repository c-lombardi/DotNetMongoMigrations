using Migrations.ToBeImplemented;
using Migrations.Types;
using Migrations.Types.ToBeImplemented;
using MongoMigrations.Implemented.Filter;
using MongoMigrations.Implemented.Filter.Experimental;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MongoMigrations.Implemented
{
    public class MigrationLocatorAsync : IMigrationLocatorAsync
    {
        protected readonly IEnumerable<Assembly> Assemblies;
        public List<MigrationFilterAsync> MigrationFilters = new List<MigrationFilterAsync>();

        public MigrationLocatorAsync(IEnumerable<Assembly> assemblies)
        {
            MigrationFilters.Add(new ExcludeExperimentalMigrationsAsync());
            Assemblies = assemblies;
        }

        protected virtual IEnumerable<MigrationAsync> GetMigrationsFromAssembly(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes()
                    .Where(t => typeof(MigrationAsync).IsAssignableFrom(t) && !t.IsAbstract)
                    .Select(Activator.CreateInstance)
                    .OfType<MigrationAsync>()
                    .Where(m => !MigrationFilters.Any(f => f.ExcludeAsync(m).Result));
            }
            catch (Exception exception)
            {
                throw new MigrationException(exception, null);
            }
        }

        Task<IEnumerable<MigrationAsync>> IMigrationLocatorAsync.GetAllMigrationsAsync()
        {
            return Task.FromResult(Assemblies
                .SelectMany(GetMigrationsFromAssembly));
        }
    }
}
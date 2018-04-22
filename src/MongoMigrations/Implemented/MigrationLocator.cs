using Migrations.ToBeImplemented;
using Migrations.Types;
using MongoMigrations.Implemented.Filter;
using MongoMigrations.Implemented.Filter.Experimental;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MongoMigrations.Implemented
{
    public class MigrationLocator : IMigrationLocator
    {
        protected readonly IEnumerable<Assembly> Assemblies;
        public List<MigrationFilter> MigrationFilters = new List<MigrationFilter>();

        public MigrationLocator(IEnumerable<Assembly> assemblies)
        {
            MigrationFilters.Add(new ExcludeExperimentalMigrations());
            Assemblies = assemblies;
        }

        protected virtual IEnumerable<Migration> GetMigrationsFromAssembly(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes()
                    .Where(t => typeof(Migration).IsAssignableFrom(t) && !t.IsAbstract)
                    .Select(Activator.CreateInstance)
                    .OfType<Migration>()
                    .Where(m => !MigrationFilters.Any(f => f.Exclude(m)));
            }
            catch (Exception exception)
            {
                throw new MigrationException(exception, null);
            }
        }

        IEnumerable<Migration> IMigrationLocator.GetAllMigrations()
        {
            return Assemblies
                .SelectMany(GetMigrationsFromAssembly);
        }
    }
}
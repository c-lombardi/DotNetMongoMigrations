using System;
using MongoMigrations;
using Migrations.Types;
using System.Collections.Generic;
using System.Reflection;

namespace RunMongoMigrations
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: RunMongoMigrations server[:port] databaseName migrationAssembly");
                return 1;
            }

            var server = args[0];
            var database = args[1];
            var migrationsAssembly = args[2];
            
            try
            {
                MigrationRunner.UpdateToLatest("mongodb://" + server + "/" + database, new List<Assembly> { Assembly.GetExecutingAssembly() });
                return 0;
            }
            catch (MigrationException e)
            {
                Console.WriteLine("Migrations Failed: " + e);
                Console.WriteLine(args[0], args[1], args[2]);
                return 1;
            }
        }
    }
}

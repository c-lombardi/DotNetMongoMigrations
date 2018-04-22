using System;

namespace MongoMigrations.Implemented.Filter.Experimental
{
    [AttributeUsage(
        AttributeTargets.Class, 
        AllowMultiple = false, 
        Inherited = true)]
    public class ExperimentalAttribute : Attribute
    {
    }
}
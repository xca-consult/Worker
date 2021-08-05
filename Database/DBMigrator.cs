using System;
using System.Reflection;
using DbUp;
using DbUp.Engine;

namespace Database
{
    public static class DbMigrator
    {
        public static DatabaseUpgradeResult Migrate(string connectionString)
        {
            EnsureDatabase.For.SqlDatabase(connectionString);

            var upgrader =
                DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToConsole()
                    .Build();

            try
            {
                var result = upgrader.PerformUpgrade();
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e);
                throw;
            }
        }
    }
}

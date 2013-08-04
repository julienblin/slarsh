namespace Slarsh.NHibernate.Tests
{
    using System.Collections.Generic;
    using System.IO;

    using Common.Logging;

    using global::NHibernate.Cfg;

    using NUnit.Framework;

    using Slarsh.NHibernate.Tests.Entities;

    [SetUpFixture]
    public class SetupFixture
    {
        public const string TestDbFile = "test.db";

        public const string ConnectionString = "Data Source=" + TestDbFile + ";Version=3;New=True;";

        public static IContextProviderFactory CreateNHContextProviderFactory()
        {
            return new NHContextProviderFactory(
                new NHContextProviderFactoryConfiguration
                {
                    DatabaseType = DatabaseType.SqLite,
                    ConnectionString = ConnectionString,
                    MappingAssemblies = new[] { typeof(Employee).Assembly },
                    AutoUpdateSchemaOnStart = true,
                    NHProperties = new Dictionary<string, string>
                        {
                            { Environment.ShowSql, "true" },
                            { Environment.FormatSql, "true" }
                        }
                });
        }

        [SetUp]
        public void SetUp()
        {
            LogManager.Adapter = new Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter(LogLevel.Debug, true, true, true, "yyyyMMdd-hh:mm:ss");
            if (File.Exists(TestDbFile))
            {
                File.Delete(TestDbFile);
            }

            ContextFactory.Start(new ContextFactoryConfiguration(CreateNHContextProviderFactory()));
        }

        [TearDown]
        public void TearDown()
        {
            ContextFactory.Current.Dispose();
        }
    }
}

﻿namespace Slarsh.NHibernate.Tests
{
    using System.Collections.Generic;

    using FluentAssertions;

    using NUnit.Framework;

    using Slarsh.NHibernate.Tests.Entities;

    using NHProperty = global::NHibernate.Cfg.Environment;

    [TestFixture]
    public class SimpleTest
    {
        [Test]
        public void It_should_perform_a_basic_persistance_test()
        {
            var nhContextProviderFactory = new NHContextProviderFactory(
                new NHContextProviderFactoryConfiguration
                    {
                        DatabaseType = DatabaseType.SqLite,
                        ConnectionString = "Data Source=test.db;Version=3;New=True;",
                        MappingAssemblies = new[] { typeof(SimpleEntity).Assembly },
                        AutoUpdateSchemaOnStart = true,
                        NHProperties = new Dictionary<string, string>
                        {
                            { NHProperty.ShowSql, "true" },
                            { NHProperty.FormatSql, "true" }
                        }
                    });
            using (var contextFactory = ContextFactory.Start(new ContextFactoryConfiguration(nhContextProviderFactory)))
            using (var context = contextFactory.StartNewContext())
            {
                var simpleEntity = new SimpleEntity { Name = "Foo" };
                context.Add(simpleEntity);

                var entity = context.Get<SimpleEntity>(simpleEntity.Id);

                entity.Should().Be(simpleEntity);

                context.Commit();
            }
        }
    }
}

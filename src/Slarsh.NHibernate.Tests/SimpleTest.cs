namespace Slarsh.NHibernate.Tests
{
    using FluentAssertions;

    using FluentNHibernate.Automapping;

    using NUnit.Framework;

    using Slarsh.NHibernate.Tests.Entities;

    [TestFixture]
    public class SimpleTest
    {
        [Test]
        public void It_should_perform_a_basic_persistance_test_inmemory()
        {
            var nhContextProviderFactory = new NHContextProviderFactory(
                new NHContextProviderFactoryConfiguration
                    {
                        MappingConfiguration = m => m.AutoMappings.Add(AutoMap.AssemblyOf<SimpleTest>()
                            .Where(x => x == typeof(SimpleEntity))),
                        FormatSql = true,
                        ShowSql = true,
                        AutoUpdateSchema = true
                    });
            using (var contextFactory = new ContextFactory())
            {
                contextFactory.Add(nhContextProviderFactory);
                contextFactory.Start();

                using (var context = contextFactory.CreateContext())
                {
                    context.Start();

                    var simpleEntity = new SimpleEntity { Name = "Foo" };
                    context.Add(simpleEntity);

                    ((NHContextProvider)context.GetContextProviderFor(simpleEntity.GetType())).Flush();

                    var entity = context.Get<SimpleEntity>(simpleEntity.Id);

                    entity.Should().Be(simpleEntity);

                    context.Rollback();
                }
            }
        }
    }
}

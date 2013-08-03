namespace Slarsh.NHibernate.Tests
{
    using System;

    using FluentAssertions;

    using NUnit.Framework;

    using Slarsh.NHibernate.Tests.Commands;
    using Slarsh.NHibernate.Tests.Entities;

    [TestFixture]
    public class AsyncTest
    {
        [Test]
        public void It_should_execute_command_async()
        {
            using (var contextFactory = ContextFactory.Start(new ContextFactoryConfiguration(SetupFixture.CreateNHContextProviderFactory())))
            using (var context = contextFactory.StartNewContext())
            {
                var command = new CreateSimpleEntityCommand { Name = "Foo" };
                var task = context.ExecuteAsync(command);

                task.Wait();

                task.Result.Should().NotBe(Guid.Empty);
            }
        }
    }
}

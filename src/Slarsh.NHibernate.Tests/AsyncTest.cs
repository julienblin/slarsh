namespace Slarsh.NHibernate.Tests
{
    using System;

    using FluentAssertions;

    using NUnit.Framework;

    using Slarsh.NHibernate.Tests.Commands;

    [TestFixture]
    public class AsyncTest
    {
        [Test]
        public void It_should_execute_command_async()
        {
            using (var context = ContextFactory.StartNewContext())
            {
                var command = new CreateSimpleEntityCommand { Name = "Foo" };
                var task = context.ExecuteAsync(command);

                task.Wait();

                task.Result.Should().NotBe(Guid.Empty);
            }
        }
    }
}

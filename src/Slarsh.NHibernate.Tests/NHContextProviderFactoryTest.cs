namespace Slarsh.NHibernate.Tests
{
    using System.ComponentModel.DataAnnotations;

    using FluentAssertions;

    using NUnit.Framework;

    [TestFixture]
    public class NHContextProviderFactoryTest
    {
        [Test]
        public void It_should_validate_connection_string()
        {
            var nhContextProviderFactory = new NHContextProviderFactory(
                new NHContextProviderFactoryConfiguration
                    {
                        DatabaseType = DatabaseType.SqLite
                    });
            using (var contextFactory = new ContextFactory(nhContextProviderFactory))
            {
                contextFactory.Invoking(x => x.Start()).ShouldThrow<SlarshException>();
            }
        }
    }
}

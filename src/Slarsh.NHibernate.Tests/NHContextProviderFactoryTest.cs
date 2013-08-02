namespace Slarsh.NHibernate.Tests
{
    using System.ComponentModel.DataAnnotations;

    using FluentAssertions;

    using NUnit.Framework;

    [TestFixture]
    public class NHContextProviderFactoryTest
    {
        [Test]
        [ExpectedException(typeof(SlarshException))]
        public void It_should_validate_connection_string()
        {
            var nhContextProviderFactory = new NHContextProviderFactory(
                new NHContextProviderFactoryConfiguration
                    {
                        DatabaseType = DatabaseType.SqLite
                    });

            ContextFactory.Start(new ContextFactoryConfiguration(nhContextProviderFactory));
        }
    }
}

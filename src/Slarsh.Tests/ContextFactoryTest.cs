namespace Slarsh.Tests
{
    using FluentAssertions;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class ContextFactoryTest
    {
        [Test]
        public void It_should_start_all_context_provider_factories()
        {
            using (var contextFactory = new ContextFactory())
            {
                var contextProviderFactory1 = new Mock<IContextProviderFactory>();
                contextProviderFactory1.Setup(x => x.Start(contextFactory));
                var contextProviderFactory2 = new Mock<IContextProviderFactory>();
                contextProviderFactory2.Setup(x => x.Start(contextFactory));
                contextFactory.Add(contextProviderFactory1.Object, contextProviderFactory2.Object);

                contextFactory.Start();

                contextProviderFactory1.VerifyAll();
                contextProviderFactory2.VerifyAll();
            }
        }

        [Test]
        public void It_should_dispose_all_context_provider_factories()
        {
            var contextFactory = new ContextFactory();
            var contextProviderFactory1 = new Mock<IContextProviderFactory>();
            contextProviderFactory1.Setup(x => x.Dispose());
            var contextProviderFactory2 = new Mock<IContextProviderFactory>();
            contextProviderFactory2.Setup(x => x.Dispose());
            contextFactory.Add(contextProviderFactory1.Object, contextProviderFactory2.Object);

            contextFactory.Dispose();

            contextProviderFactory1.VerifyAll();
            contextProviderFactory2.VerifyAll();
        }

        [Test]
        public void It_should_create_context_with_all_context_provider_factories()
        {
            using (var contextFactory = new ContextFactory())
            {
                var contextProvider = new Mock<IContextProvider>();
                var contextProviderFactory1 = new Mock<IContextProviderFactory>();
                contextProviderFactory1.Setup(x => x.Start(contextFactory));
                contextProviderFactory1.Setup(x => x.CreateContextProvider()).Returns(contextProvider.Object);
                var contextProviderFactory2 = new Mock<IContextProviderFactory>();
                contextProviderFactory2.Setup(x => x.Start(contextFactory));
                contextProviderFactory2.Setup(x => x.CreateContextProvider()).Returns(contextProvider.Object);
                contextFactory.Add(contextProviderFactory1.Object, contextProviderFactory2.Object);

                contextFactory.Start();
                var context = contextFactory.CreateContext();

                contextProviderFactory1.VerifyAll();
                contextProviderFactory2.VerifyAll();

                context.ContextFactory.Should().Be(contextFactory);
            }
        }

        [Test]
        public void It_should_not_create_context_if_not_ready()
        {
            var contextFactory = new ContextFactory();

            contextFactory.Invoking(x => x.CreateContext())
                          .ShouldThrow<SlarshException>()
                          .WithMessage("started", ComparisonMode.Substring);
        }

    }
}

namespace Slarsh
{
    using System;
    using System.Collections.Generic;

    public interface IContextFactory : IDisposable
    {
        bool IsReady { get; }

        IEnumerable<IContextProviderFactory> ContextProviderFactories { get; }

        void Add(params IContextProviderFactory[] contextProviderFactories);

        void Start();

        IContext CreateContext();
    }
}

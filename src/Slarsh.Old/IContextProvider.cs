namespace Slarsh
{
    using System;

    public interface IContextProvider : IDisposable
    {
        void Add(IEntity entity);

        void Remove(IEntity entity);

        T Get<T>(object id);

        void Execute(IExecutable executable);

        T Execute<T>(IExecutable<T> executable);

        void Start();

        void Commit();

        void Rollback();

        bool TakesCareOf(Type type);
    }
}

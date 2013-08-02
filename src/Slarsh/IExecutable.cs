namespace Slarsh
{
    public interface IExecutable
    {
        void Execute(IContext context);
    }

    public interface IExecutable<T> : IExecutable
    {
        new T Execute(IContext context);
    }
}

namespace Slarsh
{
    public interface IQuery : IExecutable
    {
    }

    public interface IQuery<T> : IQuery, IExecutable<T>
    {
    }
}

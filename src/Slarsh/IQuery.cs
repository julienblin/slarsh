namespace Slarsh
{
    /// <summary>
    /// The Query interface.
    /// A query is a specialized <see cref="IExecutable"/> that allows interrogation of data repositories. 
    /// </summary>
    /// <typeparam name="T">
    /// The type of results
    /// </typeparam>
    public interface IQuery<out T> : IExecutable<T>
    {
    }
}

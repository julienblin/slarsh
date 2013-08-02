namespace Slarsh
{
    /// <summary>
    /// A specialized <see cref="IQuery{T}"/> that returns a <see cref="IPaginationResult{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of enumerated results.
    /// </typeparam>
    public interface IEnumerableQuery<out T> : IQuery<IPaginationResult<T>>
    {
    }
}

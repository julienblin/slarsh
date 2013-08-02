namespace Slarsh
{
    using System.Collections.Generic;

    public interface IEnumerableQuery<T> : IQuery<IPaginationResult<T>>
    {
    }
}

namespace Slarsh
{
    using System.Collections.Generic;

    public interface IPaginationResult
    {
        int TotalItems { get; }

        int PageSize { get; }

        int PageCount { get; }

        int CurrentPage { get; }

        bool HasPreviousPage { get; }

        bool HasNextPage { get; }

        bool IsFirstPage { get; }

        bool IsLastPage { get; }
    }

    public interface IPaginationResult<T> : IPaginationResult
    {
        IEnumerable<T> Result { get; }
    }
}

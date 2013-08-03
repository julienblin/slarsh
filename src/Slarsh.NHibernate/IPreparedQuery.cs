namespace Slarsh.NHibernate
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Allows the final transformation of a query.
    /// </summary>
    /// <typeparam name="T">
    /// The type of result.
    /// </typeparam>
    public interface IPreparedQuery<T>
    {
        /// <summary>
        /// Fetches the associated entities.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="PreparedQuery{T}"/>.
        /// </returns>
        PreparedQuery<T> Fetch(Expression<Func<T, object>> path);

        /// <summary>
        /// Adds an order by.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="orderType">
        /// The order type.
        /// </param>
        /// <returns>
        /// The <see cref="PreparedQuery{T}"/>.
        /// </returns>
        PreparedQuery<T> OrderBy(Expression<Func<T, object>> path, OrderType orderType = OrderType.Asc);

        /// <summary>
        /// Adds an order by.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="orderType">
        /// The order type.
        /// </param>
        /// <returns>
        /// The <see cref="PreparedQuery{T}"/>.
        /// </returns>
        PreparedQuery<T> OrderBy(string path, OrderType orderType = OrderType.Asc);

        /// <summary>
        /// Runs the query and returns paginated results.
        /// </summary>
        /// <param name="paginationParams">
        /// The pagination parameters.
        /// </param>
        /// <returns>
        /// The <see cref="IPaginationResult"/>.
        /// </returns>
        IPaginationResult<T> Paginate(IPaginationParams paginationParams = null);
    }
}

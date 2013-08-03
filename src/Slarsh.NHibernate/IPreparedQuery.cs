namespace Slarsh.NHibernate
{
    using System;
    using System.Collections.Generic;
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

        /// <summary>
        /// Runs the query and returns all the results.
        /// WARNING: you might want to use <see cref="Paginate"/> instead, unless you are sure
        /// that the result set is small.
        /// </summary>
        /// <returns>
        /// The complete results.
        /// </returns>
        IEnumerable<T> List();

        /// <summary>
        /// Runs the query and returns the number of rows.
        /// </summary>
        /// <returns>
        /// The number of rows.
        /// </returns>
        int Count();

        /// <summary>
        /// Returns a single instance that matches the query, or null if the query returns no results.
        /// </summary>
        /// <returns>
        /// A single instance that matches the query, or null if the query returns no results.
        /// </returns>
        T SingleOrDefault();
    }
}

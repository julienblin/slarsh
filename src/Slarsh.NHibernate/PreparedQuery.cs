namespace Slarsh.NHibernate
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using global::NHibernate;
    using global::NHibernate.Criterion;

    /// <summary>
    /// Default implementation of <see cref="IPreparedQuery{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of result.
    /// </typeparam>
    public class PreparedQuery<T> : IPreparedQuery<T>
    {
        /// <summary>
        /// The NHibernate session.
        /// </summary>
        private readonly ISession session;

        /// <summary>
        /// The query over.
        /// </summary>
        private readonly IQueryOver<T, T> queryOver;

        /// <summary>
        /// The fetch list.
        /// </summary>
        private readonly IList<Expression<Func<T, object>>> fetchList = new List<Expression<Func<T, object>>>();

        /// <summary>
        /// The order by list.
        /// </summary>
        private readonly IList<OrderByPath> orderByList = new List<OrderByPath>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PreparedQuery{T}"/> class.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="queryOver">
        /// The query over.
        /// </param>
        public PreparedQuery(ISession session, IQueryOver<T, T> queryOver)
        {
            this.session = session;
            this.queryOver = queryOver;
        }

        /// <summary>
        /// Fetches the associated entities.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="PreparedQuery{T}"/>.
        /// </returns>
        public PreparedQuery<T> Fetch(Expression<Func<T, object>> path)
        {
            this.fetchList.Add(path);
            return this;
        }

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
        public PreparedQuery<T> OrderBy(Expression<Func<T, object>> path, OrderType orderType = OrderType.Asc)
        {
            this.orderByList.Add(new OrderByPath { Path = path, OrderType = orderType });
            return this;
        }

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
        public PreparedQuery<T> OrderBy(string path, OrderType orderType = OrderType.Asc)
        {
            this.orderByList.Add(new OrderByPath { PropertyName = path, OrderType = orderType });
            return this;
        }

        /// <summary>
        /// Runs the query and returns paginated results.
        /// </summary>
        /// <param name="paginationParams">
        /// The pagination parameters.
        /// </param>
        /// <returns>
        /// The <see cref="IPaginationResult"/>.
        /// </returns>
        public IPaginationResult<T> Paginate(IPaginationParams paginationParams = null)
        {
            if (paginationParams == null)
            {
                paginationParams = new PaginationParams();
            }

            var query = this.BuildFinalQueryOver();
            var rowCount = query.ToRowCountQuery().FutureValue<int>();

            var items = query.Skip((paginationParams.CurrentPage - 1) * paginationParams.PageSize).Take(paginationParams.PageSize).Future();
            return new PaginationResult<T>
            {
                CurrentPage = paginationParams.CurrentPage,
                PageSize = paginationParams.PageSize,
                TotalItems = rowCount.Value,
                Result = items
            };
        }

        /// <summary>
        /// Finalizes the <see cref="IQueryOver"/> creation by adding fetches, orders and some.
        /// </summary>
        /// <returns>
        /// The <see cref="IQueryOver"/>.
        /// </returns>
        protected virtual IQueryOver<T, T> BuildFinalQueryOver()
        {
            var result = this.queryOver.Clone();
            foreach (var fetch in this.fetchList)
            {
                result.Fetch(fetch);
            }

            foreach (var orderByPath in this.orderByList)
            {
                if (orderByPath.Path != null)
                {
                    switch (orderByPath.OrderType)
                    {
                        case OrderType.Asc:
                            result.OrderBy(orderByPath.Path).Asc();
                            break;
                        case OrderType.Desc:
                            result.OrderBy(orderByPath.Path).Desc();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    switch (orderByPath.OrderType)
                    {
                        case OrderType.Asc:
                            result.OrderBy(Projections.Property(orderByPath.PropertyName)).Asc();
                            break;
                        case OrderType.Desc:
                            result.OrderBy(Projections.Property(orderByPath.PropertyName)).Desc();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            // We flush before executing a query to make sure the vision of the database is accurate.
            this.session.Flush();

            return result;
        }

        /// <summary>
        /// Stores order by.
        /// </summary>
        private struct OrderByPath
        {
            /// <summary>
            /// Gets or sets the property name.
            /// </summary>
            public string PropertyName { get; set; }

            /// <summary>
            /// Gets or sets the path.
            /// </summary>
            public Expression<Func<T, object>> Path { get; set; }

            /// <summary>
            /// Gets or sets the order type.
            /// </summary>
            public OrderType OrderType { get; set; }
        }
    }
}

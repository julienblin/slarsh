namespace Slarsh.NHibernate
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using global::NHibernate;
    using global::NHibernate.Criterion;
    using global::NHibernate.Impl;

    /// <summary>
    /// Default implementation of <see cref="IPreparedQuery{T}"/> for the
    /// NHibernate ICriteria API.
    /// </summary>
    /// <typeparam name="T">
    /// The type of result.
    /// </typeparam>
    public class PreparedQueryCriteria<T> : IPreparedQuery<T>
    {
        /// <summary>
        /// The NHibernate session.
        /// </summary>
        private readonly ISession session;

        /// <summary>
        /// The criteria.
        /// </summary>
        private readonly ICriteria criteria;

        /// <summary>
        /// The fetch list.
        /// </summary>
        private readonly IList<Expression<Func<T, object>>> fetchList = new List<Expression<Func<T, object>>>();

        /// <summary>
        /// The order by list.
        /// </summary>
        private readonly IList<OrderByPath> orderByList = new List<OrderByPath>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PreparedQueryCriteria{T}"/> class.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        public PreparedQueryCriteria(ISession session, ICriteria criteria)
        {
            this.session = session;
            this.criteria = criteria;
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
        public virtual IPreparedQuery<T> Fetch(Expression<Func<T, object>> path)
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
        public virtual IPreparedQuery<T> OrderBy(Expression<Func<T, object>> path, OrderType orderType = OrderType.Asc)
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
        public virtual IPreparedQuery<T> OrderBy(string path, OrderType orderType = OrderType.Asc)
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
        public virtual IPaginationResult<T> Paginate(IPaginationParams paginationParams = null)
        {
            if (paginationParams == null)
            {
                paginationParams = new PaginationParams();
            }

            var query = this.BuildFinalCriteria();

            var rowCount = ((ICriteria)query.Clone()).SetProjection(Projections.RowCount()).FutureValue<int>();

            var items =
                query.SetFirstResult((paginationParams.CurrentPage - 1) * paginationParams.PageSize)
                     .SetMaxResults(paginationParams.PageSize)
                     .Future<T>();
            return new PaginationResult<T>
            {
                CurrentPage = paginationParams.CurrentPage,
                PageSize = paginationParams.PageSize,
                TotalItems = rowCount.Value,
                Result = items
            };
        }

        /// <summary>
        /// Runs the query and returns all the results.
        /// WARNING: you might want to use <see cref="Paginate"/> instead, unless you are sure
        /// that the result set is small.
        /// </summary>
        /// <returns>
        /// The complete results.
        /// </returns>
        public virtual IEnumerable<T> List()
        {
            return this.BuildFinalCriteria().List<T>();
        }

        /// <summary>
        /// Runs the query and returns the number of rows.
        /// </summary>
        /// <returns>
        /// The number of rows.
        /// </returns>
        public virtual int Count()
        {
            return this.BuildFinalCriteria().SetProjection(Projections.RowCount()).UniqueResult<int>();
        }

        /// <summary>
        /// Returns a single instance that matches the query, or null if the query returns no results.
        /// </summary>
        /// <returns>
        /// A single instance that matches the query, or null if the query returns no results.
        /// </returns>
        public virtual T SingleOrDefault()
        {
            return this.BuildFinalCriteria().UniqueResult<T>();
        }

        /// <summary>
        /// Finalizes the <see cref="IQueryOver"/> creation by adding fetches, orders and some.
        /// </summary>
        /// <returns>
        /// The <see cref="IQueryOver"/>.
        /// </returns>
        protected virtual ICriteria BuildFinalCriteria()
        {
            var result = (ICriteria)this.criteria.Clone();
            foreach (var fetch in this.fetchList)
            {
                result.SetFetchMode(ExpressionProcessor.FindMemberExpression(fetch.Body), FetchMode.Eager);
            }

            foreach (var orderByPath in this.orderByList)
            {
                if (orderByPath.Path != null)
                {
                    switch (orderByPath.OrderType)
                    {
                        case OrderType.Asc:
                            result.AddOrder(Order.Asc(ExpressionProcessor.FindMemberExpression(orderByPath.Path)));
                            break;
                        case OrderType.Desc:
                            result.AddOrder(Order.Desc(ExpressionProcessor.FindMemberExpression(orderByPath.Path)));
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
                            result.AddOrder(Order.Asc(orderByPath.PropertyName));
                            break;
                        case OrderType.Desc:
                            result.AddOrder(Order.Desc(orderByPath.PropertyName));
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

namespace Slarsh.NHibernate
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using global::NHibernate;
    using global::NHibernate.Criterion;

    /// <summary>
    /// Base class for NHibernate queries.
    /// </summary>
    /// <typeparam name="T">
    /// The type of result.
    /// </typeparam>
    public abstract class NHQuery<T> : Slarsh.IQuery, INHQuery
    {
        /// <summary>
        /// The NHibernate session.
        /// </summary>
        private readonly ISession session;

        /// <summary>
        /// The fetch list.
        /// </summary>
        private readonly IList<Expression<Func<T, object>>> fetchList = new List<Expression<Func<T, object>>>();

        /// <summary>
        /// The order by list.
        /// </summary>
        private readonly IList<OrderByPath> orderByList = new List<OrderByPath>();

        /// <summary>
        /// Initializes a new instance of the <see cref="NHQuery{T}"/> class.
        /// </summary>
        /// <param name="session">
        /// The NHibernate session.
        /// </param>
        protected NHQuery(ISession session)
        {
            this.session = session;
        }

        /// <summary>
        /// Gets the NHibernate session.
        /// </summary>
        protected ISession Session
        {
            get
            {
                return this.session;
            }
        }

        /// <summary>
        /// Fetches the associated entities.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="NHQuery{T}"/>.
        /// </returns>
        public NHQuery<T> Fetch(Expression<Func<T, object>> path)
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
        /// The <see cref="NHQuery{T}"/>.
        /// </returns>
        public NHQuery<T> OrderBy(Expression<Func<T, object>> path, OrderType orderType = OrderType.Asc)
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
        /// The <see cref="NHQuery{T}"/>.
        /// </returns>
        public NHQuery<T> OrderBy(string path, OrderType orderType = OrderType.Asc)
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

            var query = this.BuildFinalQueryOver(this.Session);
            var rowCount = query.ToRowCountQuery().FutureValue<int>();

            var currentPage = paginationParams.CurrentPage > 0 ? paginationParams.CurrentPage : 1;
            var items = query.Skip((currentPage - 1) * paginationParams.PageSize).Take(paginationParams.PageSize).Future();
            return new PaginationResult<T>
            {
                CurrentPage = currentPage,
                PageSize = paginationParams.PageSize,
                TotalItems = rowCount.Value,
                Result = items
            };
        }

        /// <summary>
        /// Runs the query and returns the number of rows.
        /// </summary>
        /// <returns>
        /// The number of rows.
        /// </returns>
        public virtual int Count()
        {
            return this.BuildFinalQueryOver(this.Session).RowCount();
        }

        /// <summary>
        /// Must be implemented in derived classes to actually implements the selection criteria.
        /// </summary>
        /// <param name="session">
        /// The NHibernate session.
        /// </param>
        /// <returns>
        /// The <see cref="IQueryOver"/>.
        /// </returns>
        protected abstract IQueryOver<T, T> CreateQueryOver(ISession session);

        /// <summary>
        /// Finalizes the <see cref="IQueryOver"/> creation by adding fetches, orders and some.
        /// </summary>
        /// <param name="session">
        /// The NHibernate session.
        /// </param>
        /// <returns>
        /// The <see cref="IQueryOver"/>.
        /// </returns>
        protected virtual IQueryOver<T, T> BuildFinalQueryOver(ISession session)
        {
            var queryOver = this.CreateQueryOver(session);
            foreach (var fetch in this.fetchList)
            {
                queryOver.Fetch(fetch);
            }

            foreach (var orderByPath in this.orderByList)
            {
                if (orderByPath.Path != null)
                {
                    switch (orderByPath.OrderType)
                    {
                        case OrderType.Asc:
                            queryOver.OrderBy(orderByPath.Path).Asc();
                            break;
                        case OrderType.Desc:
                            queryOver.OrderBy(orderByPath.Path).Desc();
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
                            queryOver.OrderBy(Projections.Property(orderByPath.PropertyName)).Asc();
                            break;
                        case OrderType.Desc:
                            queryOver.OrderBy(Projections.Property(orderByPath.PropertyName)).Desc();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            // We flush before executing a query to make sure the vision of the database is accurate.
            this.Session.Flush();

            return queryOver;
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

namespace Slarsh.NHibernate
{
    using global::NHibernate;

    public abstract class NHEnumerableQuery<T> : IEnumerableQuery<T>, INHQuery
    {
        private IPaginationParams paginationParams;

        public IPaginationParams PaginationParams
        {
            get
            {
                return this.paginationParams
                       ?? (this.paginationParams = new PaginationParams { CurrentPage = 1, PageSize = 25 });
            }

            set
            {
                this.paginationParams = value;
            }
        }

        /// <summary>
        /// Do not invoke.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="IPaginationResult"/>.
        /// </returns>
        IPaginationResult<T> IExecutable<IPaginationResult<T>>.Execute(IContext context)
        {
            throw new SlarshException(Resources.InternalError);
        }

        /// <summary>
        /// Do not invoke.
        /// </summary>
        /// <param name="context">
        /// The current context.
        /// </param>
        void IExecutable.Execute(IContext context)
        {
            throw new SlarshException(Resources.InternalError);
        }

        object INHQuery.InternalExecute(NHContextProvider nhContextProvider)
        {
            var nhsession = nhContextProvider.NHSession;
            var query = this.CreateNHQuery(nhsession);
            var rowCount = query.ToRowCountQuery().FutureValue<int>();

            var currentPage = this.PaginationParams.CurrentPage > 0 ? this.PaginationParams.CurrentPage : 1;
            var items = query.Skip((currentPage - 1) * this.PaginationParams.PageSize).Take(this.PaginationParams.PageSize).Future();
            return new PaginationResult<T>
            {
                CurrentPage = currentPage,
                PageSize = this.PaginationParams.PageSize,
                TotalItems = rowCount.Value,
                Result = items
            };
        }

        protected abstract IQueryOver<T, T> CreateNHQuery(ISession nhsession);
    }
}

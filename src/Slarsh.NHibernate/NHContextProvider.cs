namespace Slarsh.NHibernate
{
    using System;
    using System.Diagnostics;

    using NH = global::NHibernate;

    public class NHContextProvider : IContextProvider
    {
        /// <summary>
        /// The <see cref="NHContextProviderFactory"/>.
        /// </summary>
        private readonly NHContextProviderFactory nhContextProviderFactory;

        private NH.ISession session;

        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger logger;

        public NHContextProvider(NHContextProviderFactory nhContextProviderFactory)
        {
            this.nhContextProviderFactory = nhContextProviderFactory;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="NHContextProvider"/> class. 
        /// </summary>
        ~NHContextProvider()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public ILogger Logger
        {
            get
            {
                return this.logger ?? (this.logger = DefaultLogger.Instance);
            }
        }

        public void Add(IEntity entity)
        {
            Debug.Assert(this.session != null, "this.session != null");
            this.session.Save(entity);
        }

        public void Remove(IEntity entity)
        {
            Debug.Assert(this.session != null, "this.session != null");
            this.session.Delete(entity);
        }

        public T Get<T>(object id)
        {
            Debug.Assert(this.session != null, "this.session != null");
            return this.session.Get<T>(id);
        }

        public void Execute(IExecutable executable)
        {
            throw new NotImplementedException();
        }

        public T Execute<T>(IExecutable<T> executable)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            this.session = this.nhContextProviderFactory.SessionFactory.OpenSession();
            if (this.MustNotStartTransaction())
            {
                return;
            }

            this.session.BeginTransaction(this.nhContextProviderFactory.Configuration.IsolationLevel);
        }

        public void Commit()
        {
            if (this.session == null)
            {
                throw new SlarshException(Resources.CannotCommitNoSession);
            }

            if (this.MustNotStartTransaction())
            {
                return;
            }

            if (!this.session.Transaction.IsActive)
            {
                throw new SlarshException(Resources.CannotCommitTransactionInactive);
            }

            this.session.Transaction.Commit();
        }

        public void Rollback()
        {
            if (this.session == null)
            {
                throw new SlarshException(Resources.CannotRollbackNoSession);
            }

            if (this.MustNotStartTransaction())
            {
                return;
            }

            if (!this.session.Transaction.IsActive)
            {
                throw new SlarshException(Resources.CannotRollbackTransactionInactive);
            }

            this.session.Transaction.Rollback();
        }

        public bool TakesCareOf(Type type)
        {
            return this.nhContextProviderFactory.SessionFactory.GetClassMetadata(type) != null;
        }

        public void Flush()
        {
            Debug.Assert(this.session != null, "this.session != null");
            this.session.Flush();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose appropriate resources.
        /// </summary>
        /// <param name="disposing">
        /// true if managed resources must be disposed, false otherwise.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.session != null)
                {
                    if (!this.session.Transaction.IsActive)
                    {
                        this.Logger.Warn(Resources.SessionDisposedTransactionActive);
                    }

                    this.session.Dispose();
                }
            }
        }

        /// <summary>
        /// Creates the <see cref="NH.ISession"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="NH.ISession"/>.
        /// </returns>
        protected virtual NH.ISession CreateSession()
        {
            return this.nhContextProviderFactory.SessionFactory.OpenSession();
        }

        protected virtual bool MustNotStartTransaction()
        {
            return this.nhContextProviderFactory.Configuration.DatabaseType == DatabaseType.SQLite
                    && string.IsNullOrEmpty(this.nhContextProviderFactory.Configuration.ConnectionString);
        }
    }
}

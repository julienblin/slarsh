﻿namespace Slarsh.NHibernate
{
    using System;
    using System.Diagnostics;

    using Common.Logging;

    using NH = global::NHibernate;

    /// <summary>
    /// NHibernate <see cref="IContextProvider"/>. Binds to a <see cref="NH.ISession"/>.
    /// </summary>
    public class NHContextProvider : IContextProvider
    {
        /// <summary>
        /// The <see cref="NHContextProviderFactory"/>.
        /// </summary>
        private readonly NHContextProviderFactory nhContextProviderFactory;

        /// <summary>
        /// The context.
        /// </summary>
        private readonly IContext context;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILog log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The session.
        /// </summary>
        private NH.ISession session;

        /// <summary>
        /// Initializes a new instance of the <see cref="NHContextProvider"/> class.
        /// </summary>
        /// <param name="nhContextProviderFactory">
        /// The <see cref="NHContextProviderFactory"/>.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public NHContextProvider(NHContextProviderFactory nhContextProviderFactory, IContext context)
        {
            this.nhContextProviderFactory = nhContextProviderFactory;
            this.context = context;
            this.context.TransactionStarted += this.OnContextOnTransactionStarted;
            this.context.TransactionCommitting += this.ContextOnTransactionCommitting;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="NHContextProvider"/> class. 
        /// </summary>
        ~NHContextProvider()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the <see cref="IContext"/>.
        /// </summary>
        public IContext Context
        {
            get
            {
                return this.context;
            }
        }

        /// <summary>
        /// Gets the NHibernate <see cref="NH.ISession"/>.
        /// </summary>
        public NH.ISession NHSession
        {
            get
            {
                return this.session;
            }
        }

        /// <summary>
        /// Adds an entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Add(IEntity entity)
        {
            Debug.Assert(this.session != null, "this.session != null");
            this.session.Save(entity);
        }

        /// <summary>
        /// Removes an entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Remove(IEntity entity)
        {
            Debug.Assert(this.session != null, "this.session != null");
            this.session.Delete(entity);
        }

        /// <summary>
        /// Reloads the <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Reload(IEntity entity)
        {
            Debug.Assert(this.session != null, "this.session != null");
            this.session.Refresh(entity);
        }

        /// <summary>
        /// Gets an entity by its id.
        /// </summary>
        /// <param name="id">
        /// The entity id.
        /// </param>
        /// <typeparam name="T">
        /// The type of entity.
        /// </typeparam>
        /// <returns>
        /// The entity.
        /// </returns>
        public T Get<T>(object id)
        {
            Debug.Assert(this.session != null, "this.session != null");
            return this.session.Get<T>(id);
        }

        /// <summary>
        /// Fulfill a <see cref="IQuery{T}"/>.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <typeparam name="T">
        /// The type or returned result query.
        /// </typeparam>
        /// <returns>
        /// The result.
        /// </returns>
        public T Fulfill<T>(IQuery<T> query)
        {
            if (!(query is INHQuery))
            {
                throw new SlarshException(Resources.InternalError);
            }

            return (T)((INHQuery)query).Fulfill(this, this.NHSession);
        }

        /// <summary>
        /// Indicates whether this context provider takes care of the <paramref name="type"/>.
        /// Accepted types: <see cref="NHEntity"/>.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// True if it can handle it, false otherwise.
        /// </returns>
        public bool TakesCareOf(Type type)
        {
            return typeof(NHEntity).IsAssignableFrom(type) || typeof(INHQuery).IsAssignableFrom(type);
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
                    this.session.Dispose();
                }
            }
        }

        /// <summary>
        /// Starts the session after the transaction has started.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The arguments.
        /// </param>
        protected virtual void OnContextOnTransactionStarted(object sender, EventArgs e)
        {
            this.session = this.nhContextProviderFactory.SessionFactory.OpenSession();
            this.log.Debug(Resources.SessionOpened.Format(this.session));
        }

        /// <summary>
        /// Flush the session before the transaction commits.
        /// Not optimal, but the result is a behavior to the database
        /// closer to what is expected.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The arguments.
        /// </param>
        protected virtual void ContextOnTransactionCommitting(object sender, EventArgs e)
        {
            if (this.session != null)
            {
                this.session.Flush();
            }
        }
    }
}

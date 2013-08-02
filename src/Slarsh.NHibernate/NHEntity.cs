namespace Slarsh.NHibernate
{
    using System;

    /// <summary>
    /// Base class for NHibernate-managed entities.
    /// </summary>
    public abstract class NHEntity : IEntity
    {
#pragma warning disable 649
        /// <summary>
        /// The id.
        /// </summary>
        private Guid id;
#pragma warning restore 649

        /// <summary>
        /// Gets the id.
        /// </summary>
        public virtual Guid Id
        {
            get { return this.id; }
        }
    }
}

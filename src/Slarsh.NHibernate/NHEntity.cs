namespace Slarsh.NHibernate
{
    public abstract class NHEntity : IEntity
    {
    }

    /// <summary>
    /// Base class for NHibernate-managed entities.
    /// </summary>
    /// <typeparam name="TId">
    /// The type of ids.
    /// </typeparam>
    public abstract class NHEntity<TId> : NHEntity
    {
#pragma warning disable 649
        /// <summary>
        /// The id.
        /// </summary>
        private TId id;
#pragma warning restore 649

        /// <summary>
        /// Gets the id.
        /// </summary>
        public virtual TId Id
        {
            get { return this.id; }
        }
    }
}

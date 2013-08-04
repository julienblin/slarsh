namespace Slarsh.NHibernate
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Base class for NHibernate managed entities.
    /// </summary>
    public abstract class NHEntity : IEntity
    {
    }

    /// <summary>
    /// Base class for NHibernate-managed entities.
    /// </summary>
    /// <typeparam name="TId">
    /// The type of ids.
    /// </typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
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

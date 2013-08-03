namespace Slarsh.NHibernate
{
    using FluentNHibernate.Mapping;

    /// <summary>
    /// Base mapping class for <see cref="NHEntity"/>. Uses Fluent NHibernate mechanisms.
    /// </summary>
    /// <typeparam name="T">
    /// The <see cref="NHEntity"/> type.
    /// </typeparam>
    /// <typeparam name="TId">
    /// The type of the Id.
    /// </typeparam>
    public abstract class NHEntityMap<T, TId> : ClassMap<T>
        where T : NHEntity<TId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NHEntityMap{T,TId}"/> class.
        /// </summary>
        protected NHEntityMap()
        {
            Id(x => x.Id).Access.CamelCaseField();
        }
    }
}

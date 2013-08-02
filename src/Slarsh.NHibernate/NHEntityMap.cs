namespace Slarsh.NHibernate
{
    using FluentNHibernate.Mapping;

    /// <summary>
    /// Base mapping class for <see cref="NHEntity"/>. Uses Fluent NHibernate mechanisms.
    /// </summary>
    /// <typeparam name="T">
    /// The <see cref="NHEntity"/> type.
    /// </typeparam>
    public abstract class NHEntityMap<T> : ClassMap<T>
        where T : NHEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NHEntityMap{T}"/> class.
        /// </summary>
        protected NHEntityMap()
        {
            Id(x => x.Id).Access.CamelCaseField();
        }
    }
}

namespace Slarsh.NHibernate.Tests.Mappings
{
    using Slarsh.NHibernate.Tests.Entities;

    public class SimpleEntityMap : NHEntityMap<SimpleEntity>
    {
        public SimpleEntityMap()
        {
            this.Map(x => x.Name);
        }
    }
}

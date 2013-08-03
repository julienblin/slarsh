namespace Slarsh.NHibernate.Tests.Mappings
{
    using System;

    using Slarsh.NHibernate.Tests.Entities;

    public class SimpleEntityMap : NHEntityMap<SimpleEntity, Guid>
    {
        public SimpleEntityMap()
        {
            this.Map(x => x.Name);
        }
    }
}

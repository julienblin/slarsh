namespace Slarsh.NHibernate
{
    using System;

    public abstract class NHEntity : IEntity
    {
#pragma warning disable 649
        private Guid id;
#pragma warning restore 649

        public virtual Guid Id
        {
            get { return id; }
        }
    }
}

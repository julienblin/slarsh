namespace Slarsh.NHibernate.Tests.Entities
{
    using System;

    public class Vacancy : NHEntity<int>
    {
        public virtual DateTime StartDate { get; set; }

        public virtual DateTime? EndDate { get; set; }

        public virtual Employee Employee { get; set; }
    }
}

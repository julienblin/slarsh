namespace SampleApp.Core.Entities
{
    using Slarsh.NHibernate;

    public class Employee : NHEntity<int>
    {
        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }
    }
}

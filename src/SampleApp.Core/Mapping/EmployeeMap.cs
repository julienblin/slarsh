namespace SampleApp.Core.Mapping
{
    using SampleApp.Core.Entities;

    using Slarsh.NHibernate;

    public class EmployeeMap : NHEntityMap<Employee, int>
    {
        public EmployeeMap()
        {
            this.Map(x => x.FirstName);
            this.Map(x => x.LastName);
        }
    }
}

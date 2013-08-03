namespace SampleApp.Core.Mapping
{
    using SampleApp.Core.Entities;

    using Slarsh.NHibernate;

    public class EmployeeMap : NHEntityMap<Employee, int>
    {
        public EmployeeMap()
        {
            Map(x => x.FirstName);
            Map(x => x.LastName);
        }
    }
}

namespace Slarsh.NHibernate.Tests.Mappings
{
    using System;

    using FluentNHibernate.Mapping;

    using Slarsh.NHibernate.Tests.Entities;

    public class EmployeeMap : NHEntityMap<Employee, Guid>
    {
        public EmployeeMap()
        {
            this.Map(x => x.Name);
            this.Map(x => x.Age);

            this.HasMany(x => x.Vacancies)
               .AsSet()
               .Inverse()
               .Access.CamelCaseField()
               .KeyColumns.Add("Employee_id")
               .Cascade.AllDeleteOrphan();
        }
    }
}

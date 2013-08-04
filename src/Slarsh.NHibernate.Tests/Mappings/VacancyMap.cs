namespace Slarsh.NHibernate.Tests.Mappings
{
    using Slarsh.NHibernate.Tests.Entities;

    public class VacancyMap : NHEntityMap<Vacancy, int>
    {
        public VacancyMap()
        {
            this.Map(x => x.StartDate);
            this.Map(x => x.EndDate);

            this.References(x => x.Employee).Cascade.None();
        }
    }
}

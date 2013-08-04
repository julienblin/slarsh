namespace Slarsh.NHibernate.Tests.Entities
{
    using System;
    using System.Collections.Generic;

    public class Employee : NHEntity<Guid>
    {
        private ICollection<Vacancy> vacancies;

        public Employee()
        {
            this.vacancies = new List<Vacancy>();
        }

        public virtual string Name { get; set; }

        public virtual int? Age { get; set; }

        public virtual IEnumerable<Vacancy> Vacancies
        {
            get
            {
                return this.vacancies;
            }
        }

        public virtual void AddVacancy(Vacancy vacancy)
        {
            
        }
    }
}

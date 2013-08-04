namespace Slarsh.NHibernate.Tests.Queries
{
    using System.Collections.Generic;

    using global::NHibernate;

    using Slarsh.NHibernate.Tests.Entities;

    public class SqlQuery : NHQuerySql<IList<Employee>>
    {
        protected override IList<Employee> BuildSqlQuery(IContextProvider contextProvider, ISession session)
        {
            return
                session.CreateSQLQuery("SELECT * FROM Employee")
                       .AddEntity(typeof(Employee))
                       .List<Employee>();
        }
    }
}

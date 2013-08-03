namespace Slarsh.NHibernate.Tests.Commands
{
    using System;

    using Slarsh.NHibernate.Tests.Entities;

    public class CreateSimpleEntityCommand : ICommand<Guid>
    {
        public string Name { get; set; }

        public Guid Execute(IContext context)
        {
            var simpleEntity = new SimpleEntity { Name = this.Name };
            context.Add(simpleEntity);
            return simpleEntity.Id;
        }
    }
}

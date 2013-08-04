﻿namespace Slarsh.NHibernate.Tests.Entities
{
    using System;

    public class SimpleEntity : NHEntity<Guid>
    {
        public virtual string Name { get; set; }

        public virtual int? Age { get; set; }
    }
}

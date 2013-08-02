namespace Slarsh.NHibernate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;

    using FluentNHibernate.Cfg;
    using FluentNHibernate.Cfg.Db;

    using Slarsh.Utilities;

    using global::NHibernate.Tool.hbm2ddl;

    using NH = global::NHibernate;

    public class NHContextProviderFactory : IContextProviderFactory, IValidatableObject
    {
        /// <summary>
        /// The configuration.
        /// </summary>
        private readonly NHContextProviderFactoryConfiguration configuration;

        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger logger;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="NHContextProviderFactory"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        public NHContextProviderFactory(NHContextProviderFactoryConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="NHContextProviderFactory"/> class. 
        /// </summary>
        ~NHContextProviderFactory()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public ILogger Logger
        {
            get
            {
                return this.logger ?? (this.logger = DefaultLogger.Instance);
            }
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        [Required]
        public NHContextProviderFactoryConfiguration Configuration
        {
            get
            {
                return this.configuration;
            }
        }

        /// <summary>
        /// Gets the NHibernate configuration.
        /// </summary>
        public NH.Cfg.Configuration NHConfiguration { get; private set; }

        /// <summary>
        /// Gets the NHibernate <see cref="NH.ISessionFactory"/>.
        /// </summary>
        public NH.ISessionFactory SessionFactory { get; private set; }

        /// <summary>
        /// Determines whether the specified object is valid.
        /// </summary>
        /// <returns>
        /// A collection that holds failed-validation information.
        /// </returns>
        /// <param name="validationContext">The validation context.</param>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return this.configuration.Validate();
        }

        public virtual void Start(IContextFactory contextFactory)
        {
            this.EnforceValidation();
            this.NHConfiguration = this.BuildNHConfiguration();

            if (this.configuration.AutoUpdateSchema)
            {
                new SchemaUpdate(this.NHConfiguration).Execute(true, true);
            }

            this.SessionFactory = this.NHConfiguration.BuildSessionFactory();
        }

        public virtual IContextProvider CreateContextProvider()
        {
            Debug.Assert(this.SessionFactory != null, "this.SessionFactory != null");
            return new NHContextProvider(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose appropriate resources.
        /// </summary>
        /// <param name="disposing">
        /// true if managed resources must be disposed, false otherwise.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.SessionFactory != null)
                {
                    this.SessionFactory.Dispose();
                }
            }
        }

        protected virtual NH.Cfg.Configuration BuildNHConfiguration()
        {
            using (new StopwatchLoggerScope(this.Logger, Resources.Building, Resources.BuiltIn, Resources.NHConfiguration))
            {
                IPersistenceConfigurer fluentDbConfig;

                switch (this.configuration.DatabaseType)
                {
                    case DatabaseType.SQLite:
                        fluentDbConfig = string.IsNullOrEmpty(this.configuration.ConnectionString)
                                       ? SQLiteConfiguration.Standard.InMemory()
                                       : SQLiteConfiguration.Standard.ConnectionString(this.configuration.ConnectionString);
                        break;
                    case DatabaseType.SqlServer2008:
                        fluentDbConfig = MsSqlConfiguration.MsSql2008;
                        if (!string.IsNullOrEmpty(this.configuration.ConnectionString))
                        {
                            fluentDbConfig = ((MsSqlConfiguration)fluentDbConfig).ConnectionString(this.configuration.ConnectionString);
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var resultConfig = Fluently.Configure()
                                           .Database(fluentDbConfig)
                                           .Mappings(this.configuration.MappingConfiguration)
                                           .BuildConfiguration();

                if (this.configuration.FormatSql)
                {
                    resultConfig.SetProperty(NH.Cfg.Environment.FormatSql, "true");
                }

                if (this.configuration.ShowSql)
                {
                    resultConfig.SetProperty(NH.Cfg.Environment.ShowSql, "true");
                }

                return resultConfig;
            }
        }
    }
}

namespace Slarsh.NHibernate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;

    using Common.Logging;

    using FluentNHibernate.Cfg;
    using FluentNHibernate.Cfg.Db;

    using global::NHibernate.Tool.hbm2ddl;

    using Slarsh.Utilities;

    using NH = global::NHibernate;

    /// <summary>
    /// The NHibernate <see cref="IContextProviderFactory"/>. Binds to a <see cref="NH.ISessionFactory"/>.
    /// </summary>
    public class NHContextProviderFactory : IContextProviderFactory, IValidatableObject
    {
        /// <summary>
        /// The configuration.
        /// </summary>
        private readonly NHContextProviderFactoryConfiguration configuration;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILog log = LogManager.GetCurrentClassLogger();
        
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

        /// <summary>
        /// Starts the context provider. Will be called before any action on it.
        /// </summary>
        /// <param name="contextFactory">
        /// The context factory.
        /// </param>
        public virtual void Start(IContextFactory contextFactory)
        {
            this.EnforceValidation();
            this.NHConfiguration = this.BuildNHConfiguration();

            if (this.configuration.AutoUpdateSchemaOnStart)
            {
                new SchemaUpdate(this.NHConfiguration).Execute(true, true);
            }

            this.SessionFactory = this.NHConfiguration.BuildSessionFactory();
        }

        /// <summary>
        /// Creates a <see cref="IContextProvider"/>.
        /// </summary>
        /// <param name="context">
        /// The associated context.
        /// </param>
        /// <returns>
        /// The <see cref="IContextProvider"/>.
        /// </returns>
        public virtual IContextProvider CreateContextProvider(IContext context)
        {
            Debug.Assert(this.SessionFactory != null, "this.SessionFactory != null");
            return new NHContextProvider(this, context);
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

        /// <summary>
        /// Builds the NHibernate configuration object.
        /// </summary>
        /// <returns>
        /// The <see cref="Configuration"/>.
        /// </returns>
        protected virtual NH.Cfg.Configuration BuildNHConfiguration()
        {
            using (new StopwatchLogScope(this.log, Resources.Building, Resources.BuiltIn, Resources.NHConfiguration))
            {
                IPersistenceConfigurer fluentDbConfig;

                switch (this.configuration.DatabaseType)
                {
                    case DatabaseType.SqLite:
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

                var fluentConfig = Fluently.Configure().Database(fluentDbConfig);

                foreach (var mappingAssembly in this.configuration.MappingAssemblies)
                {
                    fluentConfig.Mappings(m => m.FluentMappings.AddFromAssembly(mappingAssembly));
                }

                var resultConfig = fluentConfig.BuildConfiguration();
                resultConfig.SetProperty("nhibernate-logger", typeof(NH.Logging.CommonLogging.CommonLoggingLoggerFactory).AssemblyQualifiedName);

                foreach (var key in this.configuration.NHProperties.Keys)
                {
                    resultConfig.SetProperty(key, this.configuration.NHProperties[key]);
                }

                return resultConfig;
            }
        }
    }
}

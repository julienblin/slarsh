namespace Slarsh.NHibernate
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Data;

    using FluentNHibernate.Cfg;

    /// <summary>
    /// Configuration parameters for <see cref="NHContextProviderFactory"/>.
    /// </summary>
    public class NHContextProviderFactoryConfiguration : IValidatable
    {
        public NHContextProviderFactoryConfiguration()
        {
            this.IsolationLevel = IsolationLevel.ReadCommitted;
            this.AutoUpdateSchema = false;
        }

        public string ConnectionString { get; set; }

        public DatabaseType DatabaseType { get; set; }

        public IsolationLevel IsolationLevel { get; set; }

        [Required]
        public Action<MappingConfiguration> MappingConfiguration { get; set; }

        /// <summary>
        /// true to format SQL correctly, false otherwise
        /// </summary>
        public bool FormatSql { get; set; }

        /// <summary>
        /// true to format SQL correctly, false otherwise
        /// </summary>
        public bool ShowSql { get; set; }

        public bool AutoUpdateSchema { get; set; }
    }
}

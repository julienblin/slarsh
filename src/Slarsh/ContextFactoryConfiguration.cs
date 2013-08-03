namespace Slarsh
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The context factory configuration parameters.
    /// </summary>
    public class ContextFactoryConfiguration : IValidatable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContextFactoryConfiguration"/> class.
        /// </summary>
        public ContextFactoryConfiguration()
        {
            this.CurrentContextHolder = new ThreadStaticCurrentContextHolder();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextFactoryConfiguration"/> class.
        /// </summary>
        /// <param name="contextProviderFactory">
        /// The unique context provider factory.
        /// </param>
        public ContextFactoryConfiguration(IContextProviderFactory contextProviderFactory)
        {
            this.ContextProviderFactories = new[] { contextProviderFactory };
            this.CurrentContextHolder = new ThreadStaticCurrentContextHolder();
        }

        /// <summary>
        /// Gets or sets the <see cref="IContextProviderFactory"/>
        /// </summary>
        [Required]
        public IEnumerable<IContextProviderFactory> ContextProviderFactories { get; set; }

        /// <summary>
        /// Gets or sets the current context holder. Default value: <see cref="ThreadStaticCurrentContextHolder"/>.
        /// </summary>
        public ICurrentContextHolder CurrentContextHolder { get; set; }
    }
}

namespace Slarsh
{
    /// <summary>
    /// Default implementation for <see cref="IPaginationParams"/>.
    /// </summary>
    public class PaginationParams : IPaginationParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationParams"/> class.
        /// </summary>
        public PaginationParams()
        {
            this.CurrentPage = 1;
            this.PageSize = 25;
        }

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        public int PageSize { get; set; }
    }
}

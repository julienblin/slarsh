namespace Slarsh
{
    /// <summary>
    /// Default implementation for <see cref="IPaginationParams"/>.
    /// </summary>
    public class PaginationParams : IPaginationParams
    {
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

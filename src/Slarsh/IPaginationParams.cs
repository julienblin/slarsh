namespace Slarsh
{
    /// <summary>
    /// Pagination parameters.
    /// </summary>
    public interface IPaginationParams
    {
        /// <summary>
        /// Gets or sets the current page. 1 is the first page.
        /// </summary>
        int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        int PageSize { get; set; }
    }
}

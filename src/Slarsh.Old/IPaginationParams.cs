namespace Slarsh
{
    public interface IPaginationParams
    {
        int CurrentPage { get; set; }

        int PageSize { get; set; }
    }
}
